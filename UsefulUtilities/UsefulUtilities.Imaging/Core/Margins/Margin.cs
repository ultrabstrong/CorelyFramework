using System;
using System.Xml.Serialization;

namespace UsefulUtilities.Imaging.Core.Margins
{
    [Serializable]
    public abstract class Margin
    {
        #region Constructor

        /// <summary>
        /// Empty constructor for serialization
        /// </summary>
        public Margin() { }

        /// <summary>
        /// Construct for individual values
        /// </summary>
        /// <param name="top"></param>
        /// <param name="bottom"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="type"></param>
        public Margin(double top, double bottom, double left, double right)
        {
            Top = top;
            Bottom = bottom;
            Left = left;
            Right = right;
        }

        /// <summary>
        /// Construct for tandb and landr margins
        /// </summary>
        /// <param name="margintopandbottom"></param>
        /// <param name="marginleftandright"></param>
        /// <param name="type"></param>
        public Margin(double margintopandbottom, double marginleftandright) : this(margintopandbottom, margintopandbottom, marginleftandright, marginleftandright) { }

        /// <summary>
        /// Construct for all margins
        /// </summary>
        /// <param name="margin"></param>
        public Margin(double margin) : this(margin, margin, margin, margin) { }

        #endregion

        #region Properties

        /// <summary>
        /// Margin unit types
        /// </summary>
        public abstract UnitType UnitType { get; }

        /// <summary>
        /// Top margin
        /// </summary>
        public double Top { get; set; }

        /// <summary>
        /// Bottom margin
        /// </summary>
        public double Bottom { get; set; }

        /// <summary>
        /// Right margin
        /// </summary>
        public double Right { get; set; }

        /// <summary>
        /// Left margin
        /// </summary>
        public double Left { get; set; }

        /// <summary>
        /// Top and Bottom margin
        /// </summary>
        [XmlIgnore]
        public double TAndB
        {
            get { return Top + Bottom; }
        }

        /// <summary>
        /// Right and Left margin
        /// </summary>
        [XmlIgnore]
        public double LAndR
        {
            get { return Right + Left; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get this object's properties
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{nameof(Top)}: {Top}, {nameof(Bottom)}: {Bottom}, {nameof(Left)}: {Left}, {nameof(Right)}: {Right}";
        }

        #endregion

    }
}
