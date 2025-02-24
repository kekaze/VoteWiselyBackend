using Microsoft.AspNetCore.Http.HttpResults;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using VoteWiselyBackend.Contracts;
using VoteWiselyBackend.Extensions;

namespace VoteWiselyBackend.Services
{
    public class DataTransformationServices
    {
        public static string PrepareString(PoliticalStance PoliticalCriteriaTitleAndValuePair)
        {
            var labelMap = new Dictionary<string, string>
            {
                { "InFavor", "In favor to" },
                { "Against", "Against" },
                { "Platforms", "Political Platform" }
                // To add other properties in the future
            };

            var sections = new List<string>();

            foreach (var prop in typeof(PoliticalStance).GetProperties())
            {
                // It is necessary to make sure that we are working with a list of strings for the below logic to work
                if (prop.PropertyType.IsListOfType(typeof(string)))
                {
                    var collection = prop.GetValue(PoliticalCriteriaTitleAndValuePair) as IEnumerable<string>;

                    if (collection?.Any() == true)
                    {
                        string label = labelMap.TryGetValue(prop.Name, out var displayLabel)
                            ? displayLabel
                            : prop.Name;

                        sections.Add($"{label}: {string.Join(", ", collection)}");
                    }
                }
            }
            return string.Join("\n", sections);
        }
    }
}
