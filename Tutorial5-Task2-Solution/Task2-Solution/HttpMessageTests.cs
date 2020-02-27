using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SSE
{
    public class HttpMessageTests
    {
        [Fact]
        public void TestResponseMessage()
        {
            var msg = new HttpMessage("HTTP/1.1 200 OK\nContent-Type: text/html\nContent-Length: 12\n\nhello world\n");
            Assert.Equal("200", msg.StatusCode);
            Assert.Equal("OK", msg.StatusMessage);
            Assert.Equal("text/html", msg.Headers["content-type"]);
            Assert.Equal("12", msg.Headers["content-length"]);
            Assert.Equal("hello world\n", msg.Content);
        }

        [Fact]
        public void TestRequestMessage()
        {
            var msg = new HttpMessage("POST /test HTTP/1.1\nHost: example.org\nContent-Length: 5\n\nhallo");
            Assert.Equal("POST", msg.Method);
            Assert.Equal("/test", msg.Resource);
            Assert.Equal("example.org", msg.Host);
            Assert.Equal("5", msg.Headers["content-length"]);
            Assert.Equal("hallo", msg.Content);
        }
    }
}
