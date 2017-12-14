﻿using EventBus.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Tests
{
    public class TestIntegrationEventHandler : IIntegrationEventHandler<TestIntegrationEvent>
    {
        public bool Handled { get; private set; }

        public TestIntegrationEventHandler()
        {
            Handled = false;
        }

        public async Task HandleAsync(TestIntegrationEvent @event)
        {
            Handled = true;
        }
    }
}
