using System.Threading;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;

namespace NetCoreDependencyInjectionIssueTests
{
	public class NetCoreDependencyInjectionIssueTests
	{
		[Test]
		public void GetService_ConstructorParameterWithDefaultValue_ShouldAlwaysBeTheDefaultValue()
		{
			using var host = Host.CreateDefaultBuilder().ConfigureWebHostDefaults(webBuilder =>
				webBuilder.ConfigureServices(sc => sc.AddTransient<ISomeClass, SomeClass>())).Build();

			using var scope = host.Services.CreateScope();

			for (var i = 1; i < 10000; i++)
			{
				var someClass = scope.ServiceProvider.GetService<ISomeClass>();
				someClass.SomeProperty.Should().Be(1, $"Run #{i}");
				Thread.Sleep(1); // With sleep fails consistently on run #3, without its most of the times somewhere between run #50 and #150
			}
		}

		public interface ISomeClass
		{
			public int SomeProperty { get; }
		}

		public class SomeClass : ISomeClass
		{
			public SomeClass(int someParameter = 1)
			{
				SomeProperty = someParameter;
			}

			public int SomeProperty { get; }
		}
	}
}