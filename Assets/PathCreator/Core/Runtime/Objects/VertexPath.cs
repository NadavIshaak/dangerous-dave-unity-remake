﻿using PathCreation.Utility;
using UnityEngine;

namespace PathCreation
{
    /// A vertex path is a collection of points (vertices) that lie along a bezier path.
    /// This allows one to do things like move at a constant speed along the path,
    /// which is not possible with a bezier path directly due to how they're constructed mathematically.
    /// This class also provides methods for getting the position along the path at a certain distance or time
    /// (where time = 0 is the start of the path, and time = 1 is the end of the path).
    /// Other info about the path (tangents, normals, rotation) can also be retrieved in this manner.
    public class VertexPath
    {
        #region Fields

        public readonly PathSpace space;
        public readonly bool isClosedLoop;
        public readonly Vector3[] localPoints;
        public readonly Vector3[] localTangents;
        public readonly Vector3[] localNormals;

        /// Percentage along the path at each vertex (0 being start of path, and 1 being the end)
        public readonly float[] times;

        /// Total distance between the vertices of the polyline
        public readonly float length;

        /// Total distance from the first vertex up to each vertex in the polyline
        public readonly float[] cumulativeLengthAtEachVertex;

        /// Bounding box of the path
        public readonly Bounds bounds;

        /// Equal to (0,0,-1) for 2D paths, and (0,1,0) for XZ paths
        public readonly Vector3 up;

        // Default values and constants:    
        private const int
            accuracy = 10; // A scalar for how many times bezier path is divided when determining vertex positions

        private const float minVertexSpacing = .01f;

        private Transform transform;

        #endregion

        #region Constructors

        /// <summary> Splits bezier path into array of vertices along the path.</summary>
        /// <param name="maxAngleError">
        ///     How much can the angle of the path change before a vertex is added. This allows fewer
        ///     vertices to be generated in straighter sections.
        /// </param>
        /// <param name="minVertexDst">Vertices won't be added closer together than this distance, regardless of angle error.</param>
        public VertexPath(BezierPath bezierPath, Transform transform, float maxAngleError = 0.3f,
            float minVertexDst = 0) :
            this(bezierPath,
                VertexPathUtility.SplitBezierPathByAngleError(bezierPath, maxAngleError, minVertexDst, accuracy),
                transform)
        {
        }

        /// <summary> Splits bezier path into array of vertices along the path.</summary>
        /// <param name="maxAngleError">
        ///     How much can the angle of the path change before a vertex is added. This allows fewer
        ///     vertices to be generated in straighter sections.
        /// </param>
        /// <param name="minVertexDst">Vertices won't be added closer together than this distance, regardless of angle error.</param>
        /// <param name="accuracy">Higher value means the change in angle is checked more frequently.</param>
        public VertexPath(BezierPath bezierPath, Transform transform, float vertexSpacing) :
            this(bezierPath,
                VertexPathUtility.SplitBezierPathEvenly(bezierPath, Mathf.Max(vertexSpacing, minVertexSpacing),
                    accuracy), transform)
        {
        }

        /// Internal contructor
        private VertexPath(BezierPath bezierPath, VertexPathUtility.PathSplitData pathSplitData, Transform transform)
        {
            this.transform = transform;
            space = bezierPath.Space;
            isClosedLoop = bezierPath.IsClosed;
            var numVerts = pathSplitData.vertices.Count;
            length = pathSplitData.cumulativeLength[numVerts - 1];

            localPoints = new Vector3[numVerts];
            localNormals = new Vector3[numVerts];
            localTangents = new Vector3[numVerts];
            cumulativeLengthAtEachVertex = new float[numVerts];
            times = new float[numVerts];
            bounds = new Bounds((pathSplitData.minMax.Min + pathSplitData.minMax.Max) / 2,
                pathSplitData.minMax.Max - pathSplitData.minMax.Min);

            // Figure out up direction for path
            up = bounds.size.z > bounds.size.y ? Vector3.up : -Vector3.forward;
            var lastRotationAxis = up;

            // Loop through the data and assign to arrays.
            for (var i = 0; i < localPoints.Length; i++)
            {
                localPoints[i] = pathSplitData.vertices[i];
                localTangents[i] = pathSplitData.tangents[i];
                cumulativeLengthAtEachVertex[i] = pathSplitData.cumulativeLength[i];
                times[i] = cumulativeLengthAtEachVertex[i] / length;

                // Calculate normals
                if (space == PathSpace.xyz)
                {
                    if (i == 0)
                    {
                        localNormals[0] = Vector3.Cross(lastRotationAxis, pathSplitData.tangents[0]).normalized;
                    }
                    else
                    {
                        // First reflection
                        var offset = localPoints[i] - localPoints[i - 1];
                        var sqrDst = offset.sqrMagnitude;
                        var r = lastRotationAxis - offset * 2 / sqrDst * Vector3.Dot(offset, lastRotationAxis);
                        var t = localTangents[i - 1] - offset * 2 / sqrDst * Vector3.Dot(offset, localTangents[i - 1]);

                        // Second reflection
                        var v2 = localTangents[i] - t;
                        var c2 = Vector3.Dot(v2, v2);

                        var finalRot = r - v2 * 2 / c2 * Vector3.Dot(v2, r);
                        var n = Vector3.Cross(finalRot, localTangents[i]).normalized;
                        localNormals[i] = n;
                        lastRotationAxis = finalRot;
                    }
                }
                else
                {
                    localNormals[i] = Vector3.Cross(localTangents[i], up) * (bezierPath.FlipNormals ? 1 : -1);
                }
            }

            // Apply correction for 3d normals along a closed path
            if (space == PathSpace.xyz && isClosedLoop)
            {
                // Get angle between first and last normal (if zero, they're already lined up, otherwise we need to correct)
                var normalsAngleErrorAcrossJoin = Vector3.SignedAngle(localNormals[localNormals.Length - 1],
                    localNormals[0], localTangents[0]);
                // Gradually rotate the normals along the path to ensure start and end normals line up correctly
                if (Mathf.Abs(normalsAngleErrorAcrossJoin) > 0.1f) // don't bother correcting if very nearly correct
                    for (var i = 1; i < localNormals.Length; i++)
                    {
                        var t = i / (localNormals.Length - 1f);
                        var angle = normalsAngleErrorAcrossJoin * t;
                        var rot = Quaternion.AngleAxis(angle, localTangents[i]);
                        localNormals[i] = rot * localNormals[i] * (bezierPath.FlipNormals ? -1 : 1);
                    }
            }

            // Rotate normals to match up with user-defined anchor angles
            if (space == PathSpace.xyz)
                for (var anchorIndex = 0; anchorIndex < pathSplitData.anchorVertexMap.Count - 1; anchorIndex++)
                {
                    var nextAnchorIndex = isClosedLoop ? (anchorIndex + 1) % bezierPath.NumSegments : anchorIndex + 1;

                    var startAngle = bezierPath.GetAnchorNormalAngle(anchorIndex) + bezierPath.GlobalNormalsAngle;
                    var endAngle = bezierPath.GetAnchorNormalAngle(nextAnchorIndex) + bezierPath.GlobalNormalsAngle;
                    var deltaAngle = Mathf.DeltaAngle(startAngle, endAngle);

                    var startVertIndex = pathSplitData.anchorVertexMap[anchorIndex];
                    var endVertIndex = pathSplitData.anchorVertexMap[anchorIndex + 1];

                    var num = endVertIndex - startVertIndex;
                    if (anchorIndex == pathSplitData.anchorVertexMap.Count - 2) num += 1;
                    for (var i = 0; i < num; i++)
                    {
                        var vertIndex = startVertIndex + i;
                        var t = i / (num - 1f);
                        var angle = startAngle + deltaAngle * t;
                        var rot = Quaternion.AngleAxis(angle, localTangents[vertIndex]);
                        localNormals[vertIndex] = rot * localNormals[vertIndex] * (bezierPath.FlipNormals ? -1 : 1);
                    }
                }
        }

        #endregion

        #region Public methods and accessors

        public void UpdateTransform(Transform transform)
        {
            this.transform = transform;
        }

        public int NumPoints => localPoints.Length;

        public Vector3 GetTangent(int index)
        {
            return MathUtility.TransformDirection(localTangents[index], transform, space);
        }

        public Vector3 GetNormal(int index)
        {
            return MathUtility.TransformDirection(localNormals[index], transform, space);
        }

        public Vector3 GetPoint(int index)
        {
            return MathUtility.TransformPoint(localPoints[index], transform, space);
        }

        /// Gets point on path based on distance travelled.
        public Vector3 GetPointAtDistance(float dst,
            EndOfPathInstruction endOfPathInstruction = EndOfPathInstruction.Loop)
        {
            var t = dst / length;
            return GetPointAtTime(t, endOfPathInstruction);
        }

        /// Gets forward direction on path based on distance travelled.
        public Vector3 GetDirectionAtDistance(float dst,
            EndOfPathInstruction endOfPathInstruction = EndOfPathInstruction.Loop)
        {
            var t = dst / length;
            return GetDirection(t, endOfPathInstruction);
        }

        /// Gets normal vector on path based on distance travelled.
        public Vector3 GetNormalAtDistance(float dst,
            EndOfPathInstruction endOfPathInstruction = EndOfPathInstruction.Loop)
        {
            var t = dst / length;
            return GetNormal(t, endOfPathInstruction);
        }

        /// Gets a rotation that will orient an object in the direction of the path at this point, with local up point along the path's normal
        public Quaternion GetRotationAtDistance(float dst,
            EndOfPathInstruction endOfPathInstruction = EndOfPathInstruction.Loop)
        {
            var t = dst / length;
            return GetRotation(t, endOfPathInstruction);
        }

        /// Gets point on path based on 'time' (where 0 is start, and 1 is end of path).
        public Vector3 GetPointAtTime(float t, EndOfPathInstruction endOfPathInstruction = EndOfPathInstruction.Loop)
        {
            var data = CalculatePercentOnPathData(t, endOfPathInstruction);
            return Vector3.Lerp(GetPoint(data.previousIndex), GetPoint(data.nextIndex), data.percentBetweenIndices);
        }

        /// Gets forward direction on path based on 'time' (where 0 is start, and 1 is end of path).
        public Vector3 GetDirection(float t, EndOfPathInstruction endOfPathInstruction = EndOfPathInstruction.Loop)
        {
            var data = CalculatePercentOnPathData(t, endOfPathInstruction);
            var dir = Vector3.Lerp(localTangents[data.previousIndex], localTangents[data.nextIndex],
                data.percentBetweenIndices);
            return MathUtility.TransformDirection(dir, transform, space);
        }

        /// Gets normal vector on path based on 'time' (where 0 is start, and 1 is end of path).
        public Vector3 GetNormal(float t, EndOfPathInstruction endOfPathInstruction = EndOfPathInstruction.Loop)
        {
            var data = CalculatePercentOnPathData(t, endOfPathInstruction);
            var normal = Vector3.Lerp(localNormals[data.previousIndex], localNormals[data.nextIndex],
                data.percentBetweenIndices);
            return MathUtility.TransformDirection(normal, transform, space);
        }

        /// Gets a rotation that will orient an object in the direction of the path at this point, with local up point along the path's normal
        public Quaternion GetRotation(float t, EndOfPathInstruction endOfPathInstruction = EndOfPathInstruction.Loop)
        {
            var data = CalculatePercentOnPathData(t, endOfPathInstruction);
            var direction = Vector3.Lerp(localTangents[data.previousIndex], localTangents[data.nextIndex],
                data.percentBetweenIndices);
            var normal = Vector3.Lerp(localNormals[data.previousIndex], localNormals[data.nextIndex],
                data.percentBetweenIndices);
            return Quaternion.LookRotation(MathUtility.TransformDirection(direction, transform, space),
                MathUtility.TransformDirection(normal, transform, space));
        }

        /// Finds the closest point on the path from any point in the world
        public Vector3 GetClosestPointOnPath(Vector3 worldPoint)
        {
            var data = CalculateClosestPointOnPathData(worldPoint);
            return Vector3.Lerp(GetPoint(data.previousIndex), GetPoint(data.nextIndex), data.percentBetweenIndices);
        }

        /// Finds the 'time' (0=start of path, 1=end of path) along the path that is closest to the given point
        public float GetClosestTimeOnPath(Vector3 worldPoint)
        {
            var data = CalculateClosestPointOnPathData(worldPoint);
            return Mathf.Lerp(times[data.previousIndex], times[data.nextIndex], data.percentBetweenIndices);
        }

        /// Finds the distance along the path that is closest to the given point
        public float GetClosestDistanceAlongPath(Vector3 worldPoint)
        {
            var data = CalculateClosestPointOnPathData(worldPoint);
            return Mathf.Lerp(cumulativeLengthAtEachVertex[data.previousIndex],
                cumulativeLengthAtEachVertex[data.nextIndex], data.percentBetweenIndices);
        }

        #endregion

        #region Internal methods

        /// For a given value 't' between 0 and 1, calculate the indices of the two vertices before and after t. 
        /// Also calculate how far t is between those two vertices as a percentage between 0 and 1.
        private TimeOnPathData CalculatePercentOnPathData(float t, EndOfPathInstruction endOfPathInstruction)
        {
            // Constrain t based on the end of path instruction
            switch (endOfPathInstruction)
            {
                case EndOfPathInstruction.Loop:
                    // If t is negative, make it the equivalent value between 0 and 1
                    if (t < 0) t += Mathf.CeilToInt(Mathf.Abs(t));
                    t %= 1;
                    break;
                case EndOfPathInstruction.Reverse:
                    t = Mathf.PingPong(t, 1);
                    break;
                case EndOfPathInstruction.Stop:
                    t = Mathf.Clamp01(t);
                    break;
            }

            var prevIndex = 0;
            var nextIndex = NumPoints - 1;
            var i = Mathf.RoundToInt(t * (NumPoints - 1)); // starting guess

            // Starts by looking at middle vertex and determines if t lies to the left or to the right of that vertex.
            // Continues dividing in half until closest surrounding vertices have been found.
            while (true)
            {
                // t lies to left
                if (t <= times[i])
                    nextIndex = i;
                // t lies to right
                else
                    prevIndex = i;
                i = (nextIndex + prevIndex) / 2;

                if (nextIndex - prevIndex <= 1) break;
            }

            var abPercent = Mathf.InverseLerp(times[prevIndex], times[nextIndex], t);
            return new TimeOnPathData(prevIndex, nextIndex, abPercent);
        }

        /// Calculate time data for closest point on the path from given world point
        private TimeOnPathData CalculateClosestPointOnPathData(Vector3 worldPoint)
        {
            var minSqrDst = float.MaxValue;
            var closestPoint = Vector3.zero;
            var closestSegmentIndexA = 0;
            var closestSegmentIndexB = 0;

            for (var i = 0; i < localPoints.Length; i++)
            {
                var nextI = i + 1;
                if (nextI >= localPoints.Length)
                {
                    if (isClosedLoop)
                        nextI %= localPoints.Length;
                    else
                        break;
                }

                var closestPointOnSegment =
                    MathUtility.ClosestPointOnLineSegment(worldPoint, GetPoint(i), GetPoint(nextI));
                var sqrDst = (worldPoint - closestPointOnSegment).sqrMagnitude;
                if (sqrDst < minSqrDst)
                {
                    minSqrDst = sqrDst;
                    closestPoint = closestPointOnSegment;
                    closestSegmentIndexA = i;
                    closestSegmentIndexB = nextI;
                }
            }

            var closestSegmentLength = (GetPoint(closestSegmentIndexA) - GetPoint(closestSegmentIndexB)).magnitude;
            var t = (closestPoint - GetPoint(closestSegmentIndexA)).magnitude / closestSegmentLength;
            return new TimeOnPathData(closestSegmentIndexA, closestSegmentIndexB, t);
        }

        public struct TimeOnPathData
        {
            public readonly int previousIndex;
            public readonly int nextIndex;
            public readonly float percentBetweenIndices;

            public TimeOnPathData(int prev, int next, float percentBetweenIndices)
            {
                previousIndex = prev;
                nextIndex = next;
                this.percentBetweenIndices = percentBetweenIndices;
            }
        }

        #endregion
    }
}