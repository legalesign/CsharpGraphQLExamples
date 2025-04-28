using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
// Required for all examples
using Amazon.CognitoIdentity;
using Amazon.CognitoIdentityProvider;
using Amazon.Extensions.CognitoAuthentication;
using Amazon.Runtime;
using System.Threading.Tasks;

/***
* This is a simple demonstration, for more about the Legalesign API contact support@legalesign.com
* dotnet add package Amazon.Extensions.CognitoAuthentication --version 2.5.6
*
* Check out the walk through of this code at https://apidocs.legalesign.com 
**/

namespace CLILegalesignExamples
{
    // These classes are intentionally empty for the purpose of this example. They are simply marker classes for the purpose of demonstration, contain no properties, and serve no other purpose.

    class CLIMyInfoExample
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Legalesign C# Command Line Sender");
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://graphql.uk.legalesign.com/graphql")
            };

            // First we need a valid security token
            string okes = await CLIMyInfoExample.GetCredsAsync(args[0], args[1]);

            httpClient.DefaultRequestHeaders.Add("User-Agent", "LegalesignCLI");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", okes);

            var queryObject = new
            {
                query = @"query { 
                user { 
                email
                firstName
                lastName
                }
            }",
                variables = new { }
            };

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Content = new StringContent(JsonSerializer.Serialize(queryObject), Encoding.UTF8, "application/json")
            };

            dynamic responseObj;

            using (var response = await httpClient.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();

                var responseString = await response.Content.ReadAsStringAsync();
                if (responseString != null)
                {
                    responseObj = JsonSerializer.Deserialize<dynamic>(responseString);
                    Console.WriteLine(responseObj);
                }
            }

            Console.ReadLine();
        }



        static async Task<string> GetCredsAsync(string username, string password)
        {
            // These are the general purpose pool and client id - if you have dedicated ones insert them here.
            var poolData = new
            {
                UserPoolId = "eu-west-2_NUPAjABy7",
                ClientId = "38kn0eb9mf2409t6mci98eqdvt",
            };

            AmazonCognitoIdentityProviderClient provider = new AmazonCognitoIdentityProviderClient(new Amazon.Runtime.AnonymousAWSCredentials());
            CognitoUserPool userPool = new CognitoUserPool(poolData.UserPoolId, poolData.ClientId, provider);
            CognitoUser user = new CognitoUser(username, poolData.ClientId, userPool, provider);
            InitiateSrpAuthRequest authRequest = new InitiateSrpAuthRequest()
            {
                Password = password
            };

            AuthFlowResponse authResponse = await user.StartWithSrpAuthAsync(authRequest).ConfigureAwait(false);
            return authResponse.AuthenticationResult.AccessToken;

        }
    }
}