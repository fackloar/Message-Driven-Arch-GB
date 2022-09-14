using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaraunt.Kitchen.Tests
{
    public class TestWriter : TextWriter
    {
        private readonly ITestOutputHelper _output;
        public override Encoding Encoding { get; } = Encoding.UTF8;

        public TestWriter(ITestOutputHelper output)
        {
            _output = output;
        }

        public override void WriteLine(string? value)
        {
            _output.WriteLine(value);
        }
    }
