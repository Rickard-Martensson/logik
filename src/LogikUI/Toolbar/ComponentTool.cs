﻿using Cairo;
using LogikUI.Circuit;
using LogikUI.Component;
using LogikUI.Simulation.Gates;
using LogikUI.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogikUI.Toolbar
{
    class ComponentTool<T> : BasicTool where T : struct, IInstance
    {
        public T BaseComponent;
        public bool PlacingComponent = false;
        public Vector2i StartPosition;
        public Vector2i VisualPosition;
        public Orientation CompOrientation;

        public ComponentTool(T exampleInstance, CircuitEditor circuitEditor) : base(Icon.AndGate(), exampleInstance.Name, circuitEditor)
        {
            BaseComponent = exampleInstance;
        }

        public override void Select(CircuitEditor editor)
        {
            PlacingComponent = true;
            editor.DrawingArea.QueueDraw();
        }

        public override void DeSelect(CircuitEditor editor)
        {
        }

        // FIXME: Keyboard and general mouse movement events!!

        public override void GestureStart(CircuitEditor editor, Vector2d dragStartPos)
        {
            StartPosition = editor.RoundToGrid(dragStartPos);
            VisualPosition = StartPosition;
            CompOrientation = Circuit.Orientation.East;

            editor.DrawingArea.QueueDraw();
        }

        public override void GestureUpdate(CircuitEditor editor, Vector2d offset)
        {
            VisualPosition = StartPosition + editor.RoundDistToGrid(offset);

            editor.DrawingArea.QueueDraw();
        }

        public override void GestureEnd(CircuitEditor editor, Vector2d endOffset)
        {
            VisualPosition = StartPosition + editor.RoundDistToGrid(endOffset);
            StartPosition = VisualPosition;

            var transaction = editor.Gates.CreateAddGateTrasaction(
                BaseComponent.Create(VisualPosition, CompOrientation)
                //new Circuit.AndGate(VisualPosition, CompOrientation)
                );

            editor.Gates.ApplyTransaction(transaction);
            editor.Transactions.PushTransaction(transaction);

            VisualPosition = StartPosition = Vector2i.Zero;

            editor.DrawingArea.QueueDraw();
            // FIXME: Add the component!
        }

        public override void Draw(CircuitEditor editor, Context cr)
        {
            if (PlacingComponent)
            {
                // FIXME: Render this will a bit of transaprency
                Gates.AndGate(cr, new Circuit.AndGate(VisualPosition, CompOrientation));
            }
        }
    }
}
