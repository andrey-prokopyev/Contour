﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageExchangeFilterInvoker.cs" company="">
//   
// </copyright>
// <summary>
//   The message exchange filter invoker.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Contour.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// The message exchange filter invoker.
    /// </summary>
    public class MessageExchangeFilterInvoker
    {
        #region Fields

        /// <summary>
        /// The _filter enumerator.
        /// </summary>
        private readonly IEnumerator<IMessageExchangeFilter> filterEnumerator;

        private readonly IDictionary<Type, IMessageExchangeFilterDecorator> filterDecorators;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="MessageExchangeFilterInvoker"/>.
        /// </summary>
        /// <param name="filters">
        /// The filters.
        /// </param>
        public MessageExchangeFilterInvoker(IEnumerable<IMessageExchangeFilter> filters)
        {
            this.filterEnumerator = filters.Reverse().
                GetEnumerator();
        }

        public MessageExchangeFilterInvoker(IEnumerable<IMessageExchangeFilter> filters, IDictionary<Type, IMessageExchangeFilterDecorator> filterDecorators)
            : this(filters)
        {
            this.filterDecorators = filterDecorators;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the inner.
        /// </summary>
        public IMessageExchangeFilter Inner { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The continue.
        /// </summary>
        /// <param name="exchange">
        /// The exchange.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public Task<MessageExchange> Continue(MessageExchange exchange)
        {
            if (!this.filterEnumerator.MoveNext())
            {
                return Filter.Result(exchange);
            }

            var currentFilter = this.filterEnumerator.Current;
            var currentFilterType = currentFilter.GetType();

            if (this.filterDecorators.ContainsKey(currentFilterType))
            {
                return this.filterDecorators[currentFilterType].Process(currentFilter, exchange, this);
            }

            return currentFilter.Process(exchange, this);
        }

        /// <summary>
        /// The process.
        /// </summary>
        /// <param name="exchange">
        /// The exchange.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public virtual Task<MessageExchange> Process(MessageExchange exchange)
        {
            return this.Continue(exchange);
        }

        #endregion
    }
}
