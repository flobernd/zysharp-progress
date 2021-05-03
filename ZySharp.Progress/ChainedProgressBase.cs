﻿using System;

using ZySharp.Validation;

namespace ZySharp.Progress
{
    /// <summary>
    /// A progress handler that reports progress updates to another progress handler.
    /// </summary>
    /// <typeparam name="TInput">The type of the input progress value.</typeparam>
    /// <typeparam name="TOutput">The type of the output progress value.</typeparam>
    public abstract class ChainedProgressBase<TInput, TOutput> :
        IChainedProgress<TInput, TOutput>
    {
        private IProgress<TOutput> _nextHandler;

        public IProgress<TOutput> NextHandler
        {
            get => _nextHandler;
            set
            {
                if (_nextHandler != null)
                {
                    throw new InvalidOperationException(Resources.NextHandlerAlreadySet);
                }

                _nextHandler = value;
            }
        }

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="nextHandler">The next progress handler in the chain.</param>
        protected ChainedProgressBase(IProgress<TOutput> nextHandler)
        {
            _nextHandler = nextHandler;
        }

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="action">The action to execute when a progress value is reported.</param>
        protected ChainedProgressBase(Action<TOutput> action)
        {
            ValidateArgument.For(action, nameof(action))
                .NotNull();

            _nextHandler = new LambdaProgress<TOutput>(action);
        }

        protected ChainedProgressBase()
        {
        }

        public abstract void Report(TInput value);

        protected void ReportNext(TOutput value)
        {
            if (_nextHandler is null)
            {
                throw new InvalidOperationException(Resources.NextHandlerNotSet);
            }

            _nextHandler.Report(value);
        }
    }
}