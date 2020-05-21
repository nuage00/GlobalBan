using System;
using JetBrains.Annotations;
using Newtonsoft.Json;
using RestSharp;
using Rocket.Core.Logging;

namespace fr34kyn01535.GlobalBan.API
{
    public static class Discord
    {
        /// <summary>
        ///     Sends a Rest.POST message to the specified url (Designed for discord URLs).
        /// </summary>
        /// <param name="url">The target URL to send the message to.</param>
        /// <param name="message">The message to be included in the Rest.POST query.</param>
        public static void SendWebhookPost(string url, object message)
        {
            try
            {
                var client = new RestClient(url);
                var request = new RestRequest(Method.POST);

                var jsonToSend = JsonConvert.SerializeObject(message);

                request.AddParameter("application/json; charset=utf-8", jsonToSend, ParameterType.RequestBody);
                request.RequestFormat = DataFormat.Json;

                client.ExecuteAsync(request, (response, handle) => { });
            }
            catch (Exception ex)
            {
                var old = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"An error occured whilst trying to send a webhook message: {ex}");
                Console.ForegroundColor = old;
                Logger.ExternalLog(ex, ConsoleColor.Red);
            }
        }

        /// <summary>
        ///     Builds a basic discord embed message.
        /// </summary>
        /// <param name="title">The message for the embed.</param>
        /// <param name="description">The description to be shown in the embed.</param>
        /// <param name="username">The username to be used in the embed.</param>
        /// <param name="avatar">The avatar to be used in the embed.</param>
        /// <param name="embedColour">The colour to be used in the embed.</param>
        /// <param name="fields">The extra fields in the message of the embed.</param>
        /// <returns>JSON Styled object to then be sent to discord by a webhook.</returns>
        [NotNull]
        public static object BuildDiscordEmbed(string title, string description, string username, string avatar,
            int embedColour, object[] fields)
        {
            var embMsg = new
            {
                title,
                description,
                color = embedColour,
                fields
            };
            var msg = new
            {
                embeds = new[] {embMsg},
                username,
                avatar_url = avatar
            };

            return msg;
        }

        /// <summary>
        ///     Builds a basic EmbedField object
        /// </summary>
        /// <param name="name">The name of the field.</param>
        /// <param name="text">The main text of the field.</param>
        /// <param name="inline">If the field should inline.</param>
        /// <returns></returns>
        [NotNull]
        public static object BuildDiscordField(string name, string text, bool? inline)
        {
            return new
            {
                name,
                value = text,
                inline
            };
        }
    }
}