using System;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.Linq;

namespace nCode.ProviderModel
{
    public class ProviderCollection<T> : ProviderCollection, IEnumerable<T> where T : ProviderBase
    {
        private List<T> orderedList;

        public ProviderCollection()
        {
            orderedList = new List<T>();
        }
        
        #region Properties
        new public T this[string name]
        {
            get { return (T)base[name]; }
        }
        #endregion

        #region Methods
        public override void Add(ProviderBase provider)
        {
            if (provider == null)
                throw new ArgumentNullException("provider", "Provider can not be null.");
            if (!(provider is T))
                throw new ArgumentException("The provider must by of type '" + typeof(T).ToString() + "'.", "provider");

            base.Add(provider);
            orderedList.Add((T)provider);
        }

        public void CopyTo(T[] array, int index)
        {
            base.CopyTo(array, index);
        }
        #endregion

        #region IEnumerable<T> Members

        public new IEnumerator<T> GetEnumerator()
        {
            return orderedList.GetEnumerator();
        }

        #endregion
    }
}
