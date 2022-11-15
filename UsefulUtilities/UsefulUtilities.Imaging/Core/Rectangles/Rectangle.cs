using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsefulUtilities.Imaging.Core
{
    [Serializable]
    public abstract class Rectangle
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public Rectangle() { }

        /// <summary>
        /// Easy constructor
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Rectangle(int width, int height)
        {
            Width = width;
            Height = height;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Rectange unit type
        /// </summary>
        public abstract UnitType UnitType { get; }

        /// <summary>
        /// Width
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// Height
        /// </summary>
        public double Height { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Return this object's properties
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{nameof(Width)}: {Width}, {nameof(Height)}: {Height}";
        }

        #endregion
    }
}
