using System;
using System.Collections.Generic;
using UnityEditor;
namespace GeNa.Core
{
    [InitializeOnLoad]
    public class GeNaUndoRedoEditor
    {
        static GeNaUndoRedoEditor()
        {
            UndoProManager.EnableUndoPro();
            GeNaUndoRedo.onRecordUndo -= OnRecordUndo;
            GeNaUndoRedo.onRecordUndo += OnRecordUndo;
            EditorApplication.update -= Update;
            EditorApplication.update += Update;
        }
        private static Queue<UndoEntity> undoQueue = new Queue<UndoEntity>();
        private static Queue<Action> postActionQueue = new Queue<Action>();
        private static void Update()
        {
            while (undoQueue.Count > 0)
            {
                var entity = undoQueue.Dequeue();
                RecordUndo(entity);
            }
            while (postActionQueue.Count > 0)
            {
                var action = postActionQueue.Dequeue();
                action?.Invoke();
            }
        }
        private static void ProcessEntity(UndoEntity undoEntity)
        {
            switch (undoEntity)
            {
                case UndoRecord undoRecord:
                    RecordUndoPro(undoRecord);
                    break;
                case SplineEntity splineEntity:
                    if (splineEntity.Spline != null)
                        Undo.RegisterCompleteObjectUndo(splineEntity.Spline, splineEntity.name);
                    break;
                case ResourceEntity resourceEntity:
                    if (resourceEntity.GameObject != null)
                        Undo.RegisterCreatedObjectUndo(resourceEntity.GameObject, resourceEntity.name);
                    else 
                        RecordUndoPro(resourceEntity);
                    break;
                case GameObjectEntity gameObjectEntity:
                    if(gameObjectEntity.m_gameObject != null) 
                        Undo.RegisterCreatedObjectUndo(gameObjectEntity.m_gameObject, gameObjectEntity.name);
                    break;
                case TerrainEntity terrainEntity:
                    RecordUndoPro(terrainEntity);
                    break;
            }
        }
        private static void OnRecordUndo(UndoEntity undoEntity, Action postAction)
        {
            undoQueue.Enqueue(undoEntity);
            postActionQueue.Enqueue(postAction);
        }
        public static void RecordUndoPro(UndoEntity undoEntity) => UndoProManager.RecordOperation(undoEntity.Perform, undoEntity.Undo, undoEntity.Dispose, undoEntity.name, undoEntity.mergeBefore, undoEntity.mergeAfter);
        public static void RecordUndo(UndoEntity undoEntity)
        {
            switch (undoEntity)
            {
                case UndoRecord undoRecord:
                    int group = undoRecord.Group;
                    ProcessEntity(undoRecord);
                    Undo.CollapseUndoOperations(group);
                    break;
                default:
                    ProcessEntity(undoEntity);
                    break;
            }
        }
    }
}