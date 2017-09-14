using EventBus.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Tests
{
    public class TestDynamicIntegrationEventHandler : IDynamicIntegrationEventHandler
    {
        public bool Handled { get; private set; }

        public TestDynamicIntegrationEventHandler()
        {
            Handled = false;
        }

        public async Task Handle(dynamic eventData)
        {
            Handled = true;
        }
    }
}
