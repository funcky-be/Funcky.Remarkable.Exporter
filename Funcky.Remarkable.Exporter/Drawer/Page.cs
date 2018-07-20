// -----------------------------------------------------------------------
//  <copyright file="Page.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Funcky.Remarkable.Exporter.Drawer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Page
    {
        public List<Layer> Layers { get; set; } = new List<Layer>();
    }
}