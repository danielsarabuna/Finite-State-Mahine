using System;
using System.Collections.Generic;

namespace FiniteStateMaсhine
{
    public sealed class Butler
    {
        private Dictionary<Type, IView> _views = new Dictionary<Type, IView>();

        public void Add(IView view)
        {
            _views.Add(view.GetType(), view);
        }

        public T Get<T>() where T : class, IView
        {
            if (_views.TryGetValue(typeof(T), out IView view)) return (T)view;
            throw new KeyNotFoundException($"{typeof(T)}");
        }

        public bool TryGet<T>(out T? value) where T : class, IView
        {
            value = null;
            if (_views.TryGetValue(typeof(T), out IView view))
            {
                value = (T)view;
                return true;
            }

            return false;
        }
    }
}