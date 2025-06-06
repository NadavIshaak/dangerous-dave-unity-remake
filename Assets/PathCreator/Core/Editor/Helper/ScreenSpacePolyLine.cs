﻿using System.Collections.Generic;
using PathCreation;
using PathCreation.Utility;
using UnityEditor;
using UnityEngine;

namespace PathCreationEditor
{
    public class ScreenSpacePolyLine
    {
        private const int accuracyMultiplier = 10;

        // dont allow vertices to be spaced too far apart, as screenspace-worldspace conversion can then be noticeably off
        private const float intermediaryThreshold = .2f;
        private readonly float[] cumululativeLengthWorld;

        private readonly float pathLengthWorld;

        // Stores the index in the vertices list where the start point of each segment is
        private readonly int[] segmentStartIndices;

        private readonly Transform transform;
        private readonly Vector3 transformPosition;
        private readonly Quaternion transformRotation;

        private readonly Vector3 transformScale;

        // For each point in the polyline, says which bezier segment it belongs to
        private readonly List<int> vertexToPathSegmentMap;

        public readonly List<Vector3> verticesWorld;

        private Vector2[] points;
        private bool premCamIsOrtho;

        private Vector3 prevCamPos;
        private Quaternion prevCamRot;

        public ScreenSpacePolyLine(BezierPath bezierPath, Transform transform, float maxAngleError, float minVertexDst,
            float accuracy = 1)
        {
            this.transform = transform;
            transformPosition = transform.position;
            transformRotation = transform.rotation;
            transformScale = transform.localScale;

            // Split path in vertices based on angle error
            verticesWorld = new List<Vector3>();
            vertexToPathSegmentMap = new List<int>();
            segmentStartIndices = new int[bezierPath.NumSegments + 1];

            verticesWorld.Add(bezierPath[0]);
            vertexToPathSegmentMap.Add(0);
            var prevPointOnPath = bezierPath[0];
            float dstSinceLastVertex = 0;
            var lastAddedPoint = prevPointOnPath;
            float dstSinceLastIntermediary = 0;

            for (var segmentIndex = 0; segmentIndex < bezierPath.NumSegments; segmentIndex++)
            {
                var segmentPoints = bezierPath.GetPointsInSegment(segmentIndex);
                verticesWorld.Add(segmentPoints[0]);
                vertexToPathSegmentMap.Add(segmentIndex);
                segmentStartIndices[segmentIndex] = verticesWorld.Count - 1;

                prevPointOnPath = segmentPoints[0];
                lastAddedPoint = prevPointOnPath;
                dstSinceLastVertex = 0;
                dstSinceLastIntermediary = 0;

                var estimatedSegmentLength = CubicBezierUtility.EstimateCurveLength(segmentPoints[0], segmentPoints[1],
                    segmentPoints[2], segmentPoints[3]);
                var divisions = Mathf.CeilToInt(estimatedSegmentLength * accuracy * accuracyMultiplier);
                var increment = 1f / divisions;

                for (var t = increment; t <= 1; t += increment)
                {
                    var pointOnPath = CubicBezierUtility.EvaluateCurve(segmentPoints[0], segmentPoints[1],
                        segmentPoints[2], segmentPoints[3], t);
                    var nextPointOnPath = CubicBezierUtility.EvaluateCurve(segmentPoints[0], segmentPoints[1],
                        segmentPoints[2], segmentPoints[3], t + increment);

                    // angle at current point on path
                    var localAngle = 180 - MathUtility.MinAngle(prevPointOnPath, pointOnPath, nextPointOnPath);
                    // angle between the last added vertex, the current point on the path, and the next point on the path
                    var angleFromPrevVertex = 180 - MathUtility.MinAngle(lastAddedPoint, pointOnPath, nextPointOnPath);
                    var angleError = Mathf.Max(localAngle, angleFromPrevVertex);


                    if (angleError > maxAngleError && dstSinceLastVertex >= minVertexDst)
                    {
                        dstSinceLastVertex = 0;
                        dstSinceLastIntermediary = 0;
                        verticesWorld.Add(pointOnPath);
                        vertexToPathSegmentMap.Add(segmentIndex);
                        lastAddedPoint = pointOnPath;
                    }
                    else
                    {
                        if (dstSinceLastIntermediary > intermediaryThreshold)
                        {
                            verticesWorld.Add(pointOnPath);
                            vertexToPathSegmentMap.Add(segmentIndex);
                            dstSinceLastIntermediary = 0;
                        }
                        else
                        {
                            dstSinceLastIntermediary += (pointOnPath - prevPointOnPath).magnitude;
                        }

                        dstSinceLastVertex += (pointOnPath - prevPointOnPath).magnitude;
                    }

                    prevPointOnPath = pointOnPath;
                }
            }

            segmentStartIndices[bezierPath.NumSegments] = verticesWorld.Count;

            // ensure final point gets added (unless path is closed loop)
            if (!bezierPath.IsClosed)
                verticesWorld.Add(bezierPath[bezierPath.NumPoints - 1]);
            else
                verticesWorld.Add(bezierPath[0]);

            // Calculate length
            cumululativeLengthWorld = new float[verticesWorld.Count];
            for (var i = 0; i < verticesWorld.Count; i++)
            {
                verticesWorld[i] = MathUtility.TransformPoint(verticesWorld[i], transform, bezierPath.Space);
                if (i > 0)
                {
                    pathLengthWorld += (verticesWorld[i - 1] - verticesWorld[i]).magnitude;
                    cumululativeLengthWorld[i] = pathLengthWorld;
                }
            }
        }

        private void ComputeScreenSpace()
        {
            if (Camera.current.transform.position != prevCamPos || Camera.current.transform.rotation != prevCamRot ||
                Camera.current.orthographic != premCamIsOrtho)
            {
                points = new Vector2[verticesWorld.Count];
                for (var i = 0; i < verticesWorld.Count; i++)
                    points[i] = HandleUtility.WorldToGUIPoint(verticesWorld[i]);

                prevCamPos = Camera.current.transform.position;
                prevCamRot = Camera.current.transform.rotation;
                premCamIsOrtho = Camera.current.orthographic;
            }
        }

        public MouseInfo CalculateMouseInfo()
        {
            ComputeScreenSpace();

            var mousePos = Event.current.mousePosition;
            var minDst = float.MaxValue;
            var closestPolyLineSegmentIndex = 0;
            var closestBezierSegmentIndex = 0;

            for (var i = 0; i < points.Length - 1; i++)
            {
                var dst = HandleUtility.DistancePointToLineSegment(mousePos, points[i], points[i + 1]);

                if (dst < minDst)
                {
                    minDst = dst;
                    closestPolyLineSegmentIndex = i;
                    closestBezierSegmentIndex = vertexToPathSegmentMap[i];
                }
            }

            var closestPointOnLine = MathUtility.ClosestPointOnLineSegment(mousePos,
                points[closestPolyLineSegmentIndex], points[closestPolyLineSegmentIndex + 1]);
            var dstToPointOnLine = (points[closestPolyLineSegmentIndex] - closestPointOnLine).magnitude;
            var percentBetweenVertices = dstToPointOnLine /
                                         (points[closestPolyLineSegmentIndex] - points[closestPolyLineSegmentIndex + 1])
                                         .magnitude;
            var closestPoint3D = Vector3.Lerp(verticesWorld[closestPolyLineSegmentIndex],
                verticesWorld[closestPolyLineSegmentIndex + 1], percentBetweenVertices);

            var distanceAlongPathWorld = cumululativeLengthWorld[closestPolyLineSegmentIndex] +
                                         Vector3.Distance(verticesWorld[closestPolyLineSegmentIndex], closestPoint3D);
            var timeAlongPath = distanceAlongPathWorld / pathLengthWorld;

            // Calculate how far between the current bezier segment the closest point on the line is

            var bezierSegmentStartIndex = segmentStartIndices[closestBezierSegmentIndex];
            var bezierSegmentEndIndex = segmentStartIndices[closestBezierSegmentIndex + 1];
            var bezierSegmentLength = cumululativeLengthWorld[bezierSegmentEndIndex] -
                                      cumululativeLengthWorld[bezierSegmentStartIndex];
            var distanceAlongBezierSegment = distanceAlongPathWorld - cumululativeLengthWorld[bezierSegmentStartIndex];
            var timeAlongBezierSegment = distanceAlongBezierSegment / bezierSegmentLength;

            return new MouseInfo(minDst, closestPoint3D, distanceAlongPathWorld, timeAlongPath, timeAlongBezierSegment,
                closestBezierSegmentIndex);
        }

        public bool TransformIsOutOfDate()
        {
            return transform.position != transformPosition || transform.rotation != transformRotation ||
                   transform.localScale != transformScale;
        }


        public struct MouseInfo
        {
            public readonly float mouseDstToLine;
            public readonly Vector3 closestWorldPointToMouse;
            public readonly float distanceAlongPathWorld;
            public readonly float timeOnPath;
            public readonly float timeOnBezierSegment;
            public readonly int closestSegmentIndex;


            public MouseInfo(float mouseDstToLine, Vector3 closestWorldPointToMouse, float distanceAlongPathWorld,
                float timeOnPath, float timeOnBezierSegment, int closestSegmentIndex)
            {
                this.mouseDstToLine = mouseDstToLine;
                this.closestWorldPointToMouse = closestWorldPointToMouse;
                this.distanceAlongPathWorld = distanceAlongPathWorld;
                this.timeOnPath = timeOnPath;
                this.timeOnBezierSegment = timeOnBezierSegment;
                this.closestSegmentIndex = closestSegmentIndex;
            }
        }
    }
}