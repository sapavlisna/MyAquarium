using System.Collections.Generic;
using Aquarium.Model;

namespace Aquarium
{
    public interface ITempService
    {
        void Run();
        void Stop();
        string GetTemp();
        List<Temperature> ParseResults(string result);
    }
}