using System.Net;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Customers.Api.Tests.Integration;

public class GithubApiServer : IDisposable
{
    private WireMockServer _server = null!;
    public string Url => _server.Url!;

    public void Start()
    {
        _server = WireMockServer.Start();
    }

    public void SetUpUser(string username)
    {
        _server?.Given(Request.Create()
                .WithPath($"/users/{username}")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithBody(GenerateGithubUserResponseBody(username))
                .WithHeader("content-type", "application/json,charset=utf-8")
                .WithStatusCode(200));
    }


    public void SetUpThrottledUser(string username)
    {
        _server?.Given(Request.Create()
                .WithPath($"/users/{username}")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithBody(" \"message\": \"API rate limit exceeded for 170.0.0.2 (But here's the good news: Authenticated requests get a higher rate limit. Check out the documentation for more details.)\",\r\n    \"documentation_url\": \"https://docs.github.com/rest/overview/resources-in-the-rest-api#rate-limiting\"")
                .WithHeader("content-type", "application/json,charset=utf-8")
                .WithStatusCode(403));
    }

    public  void Dispose()
    {
        _server.Stop();
        _server.Dispose();
    }
    private static string GenerateGithubUserResponseBody(string username)
    {
        return $$"""
                 
                 {
                     "login": "{{username}}",
                     "id": 136977525,
                     "node_id": "U_kgDOCCocdQ",
                     "avatar_url": "https://avatars.githubusercontent.com/u/136977525?v=4",
                     "gravatar_id": "",
                     "url": "https://api.github.com/users/{{username}}",
                     "html_url": "https://github.com/{{username}}",
                     "followers_url": "https://api.github.com/users/{{username}}/followers",
                     "following_url": "https://api.github.com/users/{{username}}/following{/other_user}",
                     "gists_url": "https://api.github.com/users/{{username}}/gists{/gist_id}",
                     "starred_url": "https://api.github.com/users/{{username}}/starred{/owner}{/repo}",
                     "subscriptions_url": "https://api.github.com/users/{{username}}/subscriptions",
                     "organizations_url": "https://api.github.com/{{username}}/orgs",
                     "repos_url": "https://api.github.com/users/{{username}}/repos",
                     "events_url": "https://api.github.com/users/{{username}}/events{/privacy}",
                     "received_events_url": "https://api.github.com/users/{{username}}/received_events",
                     "type": "User",
                     "site_admin": false,
                     "name": "Abdulwaisa Al Nuaimi",
                     "company": null,
                     "blog": "",
                     "location": "Yemen, Sana'a",
                     "email": null,
                     "hireable": true,
                     "bio": "“Software Developer | Junior .NET Developer | Full Stack Developer”",
                     "twitter_username": null,
                     "public_repos": 34,
                     "public_gists": 1,
                     "followers": 8,
                     "following": 31,
                     "created_at": "2023-06-18T13:11:21Z",
                     "updated_at": "2024-09-26T22:59:07Z"
                 }
                 """;
    }
}