using System.Text.Json.Nodes;
using CLILegalesignExamples;
using System.Text.Json;

namespace CLILegalesignExamples
{
    // These classes are intentionally empty for the purpose of this example. They are simply marker classes for the purpose of demonstration, contain no properties, and serve no other purpose.
    class CLIUpdateRecipient
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Legalesign C# Update Recipient Tool");

            // First we need a valid security token
            string token = await GraphQLLegalesign.GetCredsAsync(args[0], args[1]);

            Console.WriteLine($"Fetching recipients for document {args[2]}.");
            var data = await GraphQLLegalesign.QueryAsync(@"query getDocument($documentId:ID) {
                document(id: $documentId) {
                    id
                    recipients {
                        id
                        email
                        firstName
                        lastName
                    }
                }
                }", new { documentId = args[2] }, token);   

            Console.WriteLine(data);

            if (data == null) return;
        
            QLResponse? d = JsonSerializer.Deserialize<QLResponse>(data);
            QLRecipient? oldRecipient = d.data.document.recipients.Find(r => r.email == args[3]);

            if (oldRecipient != null)
            {
                dynamic newRecipient = new
                {
                    recipientId = oldRecipient.id,
                    email = args[4],
                    expiryDate = "2026-10-10T00:00:00.000Z",
                    firstName = args[5],
                    lastName = args[6]
                };

                // Got one! Let's update it.
                await UpdateRecipientAsync(oldRecipient, newRecipient, token);

            }
            else
            {
                Console.WriteLine($"Unable to find recipient {args[3]} on document {args[2]}.");
            }

            async Task UpdateRecipientAsync(dynamic oldRecipient, dynamic newRecipient, string token)
            {
                // Note we are using the variables parameter -- but you can code values into your query/mutation string
                // We also set it not to inform the previous recipient by email - you may want this option.
                var data = await GraphQLLegalesign.QueryAsync(@"mutation ChangeRecipient(
                    $recipientId: ID!,
                    $email: String,
                    $expiryDate: AWSDateTime,
                    $firstName: String,
                    $lastName: String
                    ) {
                    updateRecipient(
                        input: {
                    recipientId: $recipientId, 
                                        email: $email, 
                                        emailPreviousIfReplaced: false, 
                                        expiryDate: $expiryDate, 
                                        firstName: $firstName, 
                                        lastName: $lastName
                                    }
                            )
                        }
                ", newRecipient, token);

                // Let's check that it worked!
                QLMutation mut = JsonSerializer.Deserialize<QLMutation>(data);

                // Check for device locked witness etc.
                if(mut.errors != null) Console.WriteLine("Error ::" + mut.errors[0].message);
                else Console.WriteLine("Recipient updated.");
            }
        }
    }
}