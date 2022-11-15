using UsefulUtilities.Distribution;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace UsefulUtilities.UI.Models
{
    internal class DistributionValuesModel : ObservableCollection<DistributionValueModel>, INotifyPropertyChanged
    {
        #region Constructors

        /// <summary>
        /// Base contsturctor
        /// </summary>
        public DistributionValuesModel() : base()
        {
            this.CollectionChanged += (s, e) => { ValueChanged(); };
        }

        /// <summary>
        /// Constructor with collection
        /// </summary>
        /// <param name="collection"></param>
        public DistributionValuesModel(IEnumerable<DistributionValueModel> collection) : base(collection)
        {
            this.CollectionChanged += (s, e) => { ValueChanged(); };
            for (int i = 0; i < this.Count; i++)
            {
                this[i].ValueChanged += ValueChanged;
            }
        }

        /// <summary>
        /// Constructor with list
        /// </summary>
        /// <param name="list"></param>
        public DistributionValuesModel(List<DistributionValueModel> list) : base(list)
        {
            this.CollectionChanged += (s, e) => { ValueChanged(); };
            for (int i = 0; i < this.Count; i++)
            {
                this[i].ValueChanged += ValueChanged;
            }
        }

        /// <summary>
        /// Constructor with DistributionValue list
        /// </summary>
        /// <param name="list"></param>
        public DistributionValuesModel(List<DistributionValue> list) : base(list.Select(m => new DistributionValueModel(m)))
        {
        }

        /// <summary>
        /// Constructor with decimal list
        /// </summary>
        /// <param name="list"></param>
        public DistributionValuesModel(List<decimal> list) : base(list.Select(m => new DistributionValueModel(m)))
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Underlying distribution values
        /// </summary>
        public List<DistributionValue> DistributionValues
        {
            get => this.Select(m => m.DistributionValue).ToList();
        }

        /// <summary>
        /// Return CSV of distributions
        /// </summary>
        public string DistributionsCsv
        {
            get
            {
                List<string> values = new List<string>();
                for (int i = 0; i < this.Count; i++)
                {
                    values.Add(this[i].Value.ToString());
                }
                return $"{{ {string.Join(", ", values)} }}";
            }
        }

        /// <summary>
        /// Sum of distributions
        /// </summary>
        public decimal DistributionsSum
        {
            get
            {
                decimal sum = 0;
                for (int i = 0; i < this.Count; i++)
                {
                    sum += this[i].Value;
                }
                return sum;
            }
        }

        /// <summary>
        /// Text color for correct / incorrect distributoins
        /// </summary>
        public string PercentDistributionColor
        {
            get
            {
                if (DistributionsSum != 100.0M)
                {
                    // Red for under or over allocation
                    return "#FF0000";
                }
                else
                {
                    // Green for correct allocation
                    return "#43B049";
                }
            }
        }


        #endregion

        #region Methods

        /// <summary>
        /// Return decimal list
        /// </summary>
        /// <returns></returns>
        public List<decimal> ToList()
        {
            return this.Select(m => m.Value).ToList();
        }

        /// <summary>
        /// Custom add implementation
        /// </summary>
        /// <param name="item"></param>
        public new void Add(DistributionValueModel item)
        {
            item.ValueChanged += ValueChanged;
            base.Add(item);
        }

        /// <summary>
        /// Run value changed login
        /// </summary>
        public void ValueChanged()
        {
            Notify(nameof(DistributionsSum));
            Notify(nameof(DistributionsCsv));
            Notify(nameof(PercentDistributionColor));
        }

        /// <summary>
        /// Notify property changed
        /// </summary>
        public new event PropertyChangedEventHandler PropertyChanged;
        public void Notify(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion

    }
}
