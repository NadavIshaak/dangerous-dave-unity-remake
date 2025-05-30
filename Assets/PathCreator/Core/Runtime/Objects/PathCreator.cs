﻿using System;
using UnityEditor;
using UnityEngine;

namespace PathCreation
{
    public class PathCreator : MonoBehaviour
    {
        [SerializeField] [HideInInspector] private PathCreatorData editorData;
        [SerializeField] [HideInInspector] private bool initialized;

        private GlobalDisplaySettings globalEditorDisplaySettings;

        // Vertex path created from the current bezier path
        public VertexPath path
        {
            get
            {
                if (!initialized) InitializeEditorData(false);
                return editorData.GetVertexPath(transform);
            }
        }

        // The bezier path created in the editor
        public BezierPath bezierPath
        {
            get
            {
                if (!initialized) InitializeEditorData(false);
                return editorData.bezierPath;
            }
            set
            {
                if (!initialized) InitializeEditorData(false);
                editorData.bezierPath = value;
            }
        }

        /// This class stores data for the path editor, and provides accessors to get the current vertex and bezier path.
        /// Attach to a GameObject to create a new path editor.
        public event Action pathUpdated;

        #region Internal methods

        /// Used by the path editor to initialise some data
        public void InitializeEditorData(bool in2DMode)
        {
            if (editorData == null) editorData = new PathCreatorData();
            editorData.bezierOrVertexPathModified -= TriggerPathUpdate;
            editorData.bezierOrVertexPathModified += TriggerPathUpdate;

            editorData.Initialize(in2DMode);
            initialized = true;
        }

        public PathCreatorData EditorData => editorData;

        public void TriggerPathUpdate()
        {
            if (pathUpdated != null) pathUpdated();
        }

#if UNITY_EDITOR

        // Draw the path when path objected is not selected (if enabled in settings)
        private void OnDrawGizmos()
        {
            // Only draw path gizmo if the path object is not selected
            // (editor script is resposible for drawing when selected)
            var selectedObj = Selection.activeGameObject;
            if (selectedObj != gameObject)
                if (path != null)
                {
                    path.UpdateTransform(transform);

                    if (globalEditorDisplaySettings == null) globalEditorDisplaySettings = GlobalDisplaySettings.Load();

                    if (globalEditorDisplaySettings.visibleWhenNotSelected)
                    {
                        Gizmos.color = globalEditorDisplaySettings.bezierPath;

                        for (var i = 0; i < path.NumPoints; i++)
                        {
                            var nextI = i + 1;
                            if (nextI >= path.NumPoints)
                            {
                                if (path.isClosedLoop)
                                    nextI %= path.NumPoints;
                                else
                                    break;
                            }

                            Gizmos.DrawLine(path.GetPoint(i), path.GetPoint(nextI));
                        }
                    }
                }
        }
#endif

        #endregion
    }
}