using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Auth0Integration
{
    public static class save_user_in_mongodb
    {
        private static readonly Lazy<MongoClient> lazyClient = new(InitializeMongoClient);
        private static MongoClient MongoClient => lazyClient.Value;

        private static MongoClient InitializeMongoClient()
        {
            var uri = Environment.GetEnvironmentVariable("MongoDBConnectionString");
            var client = new MongoClient(uri);

            return client;
        }

        [FunctionName("save_user_in_mongodb")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var user = JsonConvert.DeserializeObject<User>(requestBody);

            try
            {
                log.LogInformation($"Trying to save user {requestBody}");

                var database = MongoClient.GetDatabase("content-management-service");
                var collection = database.GetCollection<User>("users");

                await collection.ReplaceOneAsync(x => x.UserId == user.UserId, user, new ReplaceOptions { IsUpsert = true });
            }
            catch (Exception e)
            {
                log.LogError($"Problem while saving user: {requestBody}\n{e.Message}");
                return new BadRequestObjectResult($"Problem while saving user: {requestBody}\n{e.Message}");
            }

                log.LogInformation($"Saved user: {requestBody}");
            return new OkObjectResult($"Saved user: {requestBody}");
        }
    }

    public class Identity {

        [JsonProperty("accessToken")]
        public string AccessToken { get; set; }

        [JsonProperty("connection")]
        public string Connection { get; set; }

        [JsonProperty("isSocial")]
        public bool IsSocial { get; set; }

        // [JsonProperty("profileData")]
        // public string ProfileData { get; set; }

        [JsonProperty("provider")]
        public string Provider { get; set; }

        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("user_id")]
        public string User_Id { get; set; }
    }

    public class User
    {
        // [JsonProperty("app_metadata")]
        // public string AppMetadata { get; set; }

        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("email_verified")]
        public bool EmailVerified { get; set; }

        // [JsonProperty("enrolledFactors")]
        // public List<> EnrolledFactors { get; set; }

        [JsonProperty("family_name")]
        public string FamilyName { get; set; }

        [JsonProperty("given_name")]
        public string GivenName { get; set; }

        [JsonProperty("identities")]
        public List<Identity> Identities { get; set; }

        
        [JsonProperty("last_password_reset")]
        public DateTime LastPasswordReset { get; set; }
        
        // [JsonProperty("multifactor")]
        // public List<> Multifactor { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("nickname")]
        public string Nickname { get; set; }

        [JsonProperty("phone_number")]
        public string PhoneNumber { get; set; }

        [JsonProperty("phone_verified")]
        public bool PhoneVerified { get; set; }

        [JsonProperty("picture")]
        public string Picture { get; set; }

        [JsonProperty("updated_at")]
        public string UpdatedAt { get; set; }

        [JsonProperty("user_id")]
        public string UserId { get; set; }

        // [JsonProperty("user_metadata")]
        // public string UserMetadata { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }
    }
}
