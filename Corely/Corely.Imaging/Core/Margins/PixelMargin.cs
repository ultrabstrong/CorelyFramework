using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corely.Imaging.Core.Margins
{
    [Serializable]
    public class PixelMargin : Margin
    {
        #region Constructors

        /// <summary>
        /// Empty constructor for serialization
        /// </summary>
        public PixelMargin() : base() { }

        /// <summary>
        /// Construct for individual values
        /// </summary>
        /// <param name="top"></param>
        /// <param name="bottom"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="type"></param>
        public PixelMargin(double top, double bottom, double left, double right) : base(top, bottom, left, right) { }

        /// <summary>
        /// Construct for tandb and landr margins
        /// </summary>
        /// <param name="margintopandbottom"></param>
        /// <param name="marginleftandright"></param>
        /// <param name="type"></param>
        public PixelMargin(double margintopandbottom, double marginleftandright) : base(margintopandbottom, margintopandbottom, marginleftandright, marginleftandright) { }

        /// <summary>
        /// Construct for all margins
        /// </summary>
        /// <param name="margin"></param>
        public PixelMargin(double margin) : base(margin, margin, margin, margin) { }

        #endregion

        #region Properties

        /// <summary>
        /// Unit type of margin
        /// </summary>
        public override UnitType UnitType => UnitType.Pixel;

        #endregion
    }
}
