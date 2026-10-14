using System.Net.Http;

namespace CoworkingBooking.Api.Extensions
{
    public sealed class KeycloakBackchannelHandler : DelegatingHandler
    {
        public KeycloakBackchannelHandler()
        {
            InnerHandler = new HttpClientHandler();
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var uri = request.RequestUri;

            if (uri is not null &&
                uri.Scheme == Uri.UriSchemeHttp &&
                uri.Host == "localhost" &&
                uri.Port == 7080)
            {
                request.RequestUri = new UriBuilder(uri)
                {
                    Host = "host.docker.internal"
                }.Uri;
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}