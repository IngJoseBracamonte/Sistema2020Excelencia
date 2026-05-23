using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace SistemaSatHospitalario.Tests.Unit.Common
{
    public static class MockDbSetExtensions
    {
        public static Mock<DbSet<T>> BuildMockDbSet<T>(this IQueryable<T> data) where T : class
        {
            var mockSet = new Mock<DbSet<T>>();
            
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<T>(data.Provider));
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
            
            mockSet.As<IAsyncEnumerable<T>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<T>(data.GetEnumerator()));

            mockSet.Setup(m => m.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                .Returns((object[] keyValues, CancellationToken token) =>
                {
                    var key = keyValues?.FirstOrDefault();
                    if (key == null) return ValueTask.FromResult<T?>(null);
                    
                    var item = data.AsEnumerable().FirstOrDefault(x => {
                        var prop = x.GetType().GetProperty("Id");
                        if (prop != null)
                        {
                            var val = prop.GetValue(x);
                            return val != null && val.Equals(key);
                        }
                        return false;
                    });
                    
                    return ValueTask.FromResult<T?>(item);
                });

            return mockSet;
        }
    }
}
