using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Amazon.CognitoIdentity;
using Amazon.CognitoIdentityProvider;
using Amazon.Extensions.CognitoAuthentication;
using Amazon.Runtime;
using System.Threading.Tasks;

/***
* This is a simple demonstration for more about the Legalesign API contact support@legalesign.com
* dotnet add package Amazon.Extensions.CognitoAuthentication --version 2.5.6
*
* Check out the walk through of this code at https://apidocs.legalesign.com 
**/

namespace CLILegalesignExamples
{
    class CLIUpdateRecipient
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Legalesign C# Recipient");

            // First we need a valid security token
            string token = await GraphQLLegalesign.GetCredsAsync(args[0], args[1]);

            Console.WriteLine($"Fetching recipients for document {args[2]}.")
            var data = await GraphQLLegalesign.Query(@"query getDoument($documentId:ID) {
                document(id: $documentId) {
                    id
                    recipients {
                        id
                        email
                        firstName
                        lastName
                    }
                }
                }", new { documentId: args[2] }, token)

            Console.ReadLine(data); 

            // Find a match for the recipient we've been asked to update
            string oldEmail = args[3];

            var oldRecipient = Array.Find(data.document.recipients, element => element.email == args[3])

            if(CLIUpdateRecipient !=null) {
                const newRecipient = new { recipientId: oldRecipient.id,
                    email: args[4], 
                    expiryDate: "2026-10-10T00:00:00.000Z",
                    firstName: args[5],
                    lastName: args[6]
                };

                // Got one! Let's update it.
                UpdateRecipientAsync( oldRecipient, newRecipient, token);
  

            } else {
                Console.WriteLine($"Unable to find recipient {args[3]} on document {args[2]}.")            
            } 
        };

        static async Task UpdateRecipientAsync(var oldRecipient, var newRecipient, token)
        {
            var graphQLVariables = new { recipientId: oldRecipient.id,
              email: "bert@customer.xyz", 
            expiryDate: "2026-10-10T00:00:00.000Z",
            firstName: "Bert",
            lastName: "Updatesmith"
            };

            // Note we are using the variables object -- but you can code values into your query/mutation string
            var data = await GraphQLLegalesign.Query("""mutation ChangeRecipient($recipientId: ID!) {
                updateRecipient(
                    input: { recipientId: $recipientId, email: $email, emailPreviousIfReplaced: false, expiryDate: $expiryDate, firstName: $firstName, lastName: $lastName}
                )
            } """, graphQLVariables, token)
        }
    }

}
