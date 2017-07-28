using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EventBus.Abstractions;

namespace EventBus.Test
{
    public class TestIntegrationEventHandler : IIntegrationEventHandler<TestIntegrationEvent>
    {
        public bool Handled { get; private set; }

        public TestIntegrationEventHandler()
        {
            Handled = false;
        }

        public async Task Handle(TestIntegrationEvent @event)
        {
            Handled = true;
        }
    }
}
