using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Amazon.CognitoIdentity;
using Amazon.CognitoIdentityProvider;
using Amazon.Extensions.CognitoAuthentication;
using Amazon.Runtime;
using System.Threading.Tasks;

namespace CLILegalesignExamples
{
    class GraphQLLegalesign
    {
        static async Task QueryAsync(var graphQLQuery, var graphQLVariables, token)
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://graphql.uk.legalesign.com/graphql")
            };

            httpClient.DefaultRequestHeaders.Add("User-Agent", "LegalesignCLI");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var queryObject = new
            {
                query = graphQLQuery
            },
                variables = graphQLVariables
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
                    return responseObj;
                }
            }

            return new { error: "Unable to retrieve data."}

        }



        public static async Task<string> GetCredsAsync(string username, string password)
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