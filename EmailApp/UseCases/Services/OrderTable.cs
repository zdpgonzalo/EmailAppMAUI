using System.Runtime.InteropServices;
using System.Collections;


namespace MailAppMAUI.UseCases.Services
{
    /// <summary> Tabla generica para listas de ordenes
    /// Implementa listas de ordenes entre procesos
    /// La clase esta sincronizada para uso multiproceso
    /// </summary>
    /// <typeparam name="T"> Tipo del elemento almacenado </typeparam>

    [ComVisible(false)]
    public class OrderTable<T>: IEnumerable
    {
        private List<T> OrdList;

        private int itemCount = 0;

        public OrderTable()
        {
            OrdList = new List<T>();
        }

        public int Count
        {
            get
            {
                lock (OrdList)
                {
                    return OrdList.Count;
                }

            }
        }

        public T this[int index]
        {
            get
            {
                T order = default(T);

                lock (OrdList)
                {
                    order = OrdList[index];
                }
                return order;
            }
            set
            {
                lock (OrdList)
                {
                    OrdList[index] = value;
                }
            }
        }

        public void Add(T order)
        {
            lock (OrdList)
            {
                OrdList.Add(order);
            }
        }

        public void Insert(int index, T order)
        {
            lock (OrdList)
            {
                OrdList.Insert(index, order);
            }
        }

        public void RemoveAt(int index)
        {
            lock (OrdList)
            {
                OrdList.RemoveAt(index);
            }
        }

        public void Reset()
        {
            lock (OrdList)
            {
                OrdList.Clear();
            }
        }

        public bool Contains(T order)
        {
            return (IndexOf(order) >= 0);
        }

        public int IndexOf(T order)
        {
            lock (OrdList)
            {
                return OrdList.IndexOf(order);
            }
        }

        public T CircularPeek()
        {
            T order = default(T);
            lock (OrdList)
            {
                if (OrdList.Count > 0 && itemCount < OrdList.Count)
                {
                    order = OrdList[itemCount];
                    itemCount++;
                }
                else
                {
                    order = OrdList[0]; //Siempre que se llame aqui es porque hay un elemento, devolver el del 0
                    itemCount = 0;
                }
            }
            return order;
        }

        public void CircularRemove()
        {
            lock(OrdList)
            {
                itemCount -= 1;

                OrdList.RemoveAt(itemCount);

                if (itemCount >= OrdList.Count)
                {
                    itemCount = 0;
                }
            }
        }
        public T Peek()
        {
            T order = default(T);
            lock (OrdList)
            {
                if (OrdList.Count > 0)
                    order = OrdList[0];
            }
            return order;
        }

        public void Remove()
        {
            RemoveAt(0);
        }

        public T Next()
        {
            T order = default(T);
            lock (OrdList)
            {
                if (OrdList.Count > 0)
                {
                    order = OrdList[0];
                    OrdList.RemoveAt(0);
                }
            }
            return order;
        }

        public T Next(int index )
        {
            T order = default(T);

            lock (OrdList)
            {
                if (index >= 0 && index < OrdList.Count)
                {
                    order = OrdList[index];     
                    OrdList.RemoveAt(index);    
                }
            }
            return order;
        }

        public T Next(Func<T, bool> filter)
        {
            T order = default(T);

            lock (OrdList)
            {
                var next = OrdList.FirstOrDefault(filter);

                if (next != null)
                {
                    int index = OrdList.IndexOf(next);
                    if (index >= 0 && index < OrdList.Count)
                    {
                        order = OrdList[index];
                        OrdList.RemoveAt(index);
                    }
                }
            }

            return order;
        }


        public IEnumerator GetEnumerator()
        {
            return OrdList.GetEnumerator();
        }

        public T[] Select( Func<T, bool> filter )
        {
            T[] items = null;

            lock (OrdList)
            {
                var select = OrdList.Where(filter);

                if (select != null)
                    items = select.ToArray();
            }
            return items;
        }

    }

}
