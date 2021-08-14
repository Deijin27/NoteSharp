using Notes.RouteUtil;
using Notes.Services;
using Notes.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tests.Mocks
{
    class MockNavigationService : INavigationService
    {
        public int GoBackCallCount = 0;
        public Task GoBackAsync()
        {
            GoBackCallCount++;
            return Task.CompletedTask;
        }

        public Queue<(Type, object)> GoToWithParametersCalls = new Queue<(Type, object)>();
        public Task GoToAsync<TViewModel, TSetupParameters>(TSetupParameters setupParamters)
            where TViewModel : IQueryableViewModel<TSetupParameters>
            where TSetupParameters : IQueryConvertable
        {
            GoToWithParametersCalls.Enqueue((typeof(TViewModel), setupParamters));
            return Task.CompletedTask;
        }

        public Queue<Type> GoToWithoutParametersCalls = new Queue<Type>();
        public Task GoToAsync<TViewModel>() where TViewModel : IViewModel
        {
            GoToWithoutParametersCalls.Enqueue(typeof(TViewModel));
            return Task.CompletedTask;
        }
    }
}
