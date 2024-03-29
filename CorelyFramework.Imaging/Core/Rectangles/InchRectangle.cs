﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorelyFramework.Imaging.Core.Rectangles
{
    [Serializable]
    public class InchRectange : Rectangle
    {
        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public InchRectange() : base() { }

        /// <summary>
        /// Easy constructor
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public InchRectange(int width, int height) : base(width, height) { }

        #endregion

        #region Properties

        /// <summary>
        /// Unit type of margin
        /// </summary>
        public override UnitType UnitType => UnitType.Inch; 

        #endregion
    }
}
