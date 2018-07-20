// -----------------------------------------------------------------------
//  <copyright file="Segment.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Funcky.Remarkable.Exporter.Drawer
{
    using System;
    using System.Linq;

    public class Segment
    {
        public float HorizontalPosition { get; set; }

        public float Pressure { get; set; }

        public float Speed { get; set; }

        public Stroke Stroke { get; set; }

        public float Tilt { get; set; }

        public float VerticalPosition { get; set; }
    }
}