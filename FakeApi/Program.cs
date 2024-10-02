using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

var wireMockServer = WireMockServer.Start();
Console.WriteLine($"WireMock is now running on: {wireMockServer.Urls[0]}");

// Setup WireMock response for the specified path and request type
wireMockServer
    .Given(Request.Create()
        .WithPath("/users/alnaimi-github")
        .UsingGet())
    .RespondWith(Response.Create()
        .WithHeader("Content-Type", "application/json; charset=utf-8")
        .WithBodyAsJson(new
        {
            login = "alnaimi-github",
            id = 136977525,
            node_id = "U_kgDOCCocdQ",
            avatar_url = "https://avatars.githubusercontent.com/u/136977525?v=4",
            gravatar_id = "",
            url = "https://api.github.com/users/alnaimi-github",
            html_url = "https://github.com/alnaimi-github",
            followers_url = "https://api.github.com/users/alnaimi-github/followers",
            following_url = "https://api.github.com/users/alnaimi-github/following{/other_user}",
            gists_url = "https://api.github.com/users/alnaimi-github/gists{/gist_id}",
            starred_url = "https://api.github.com/users/alnaimi-github/starred{/owner}{/repo}",
            subscriptions_url = "https://api.github.com/users/alnaimi-github/subscriptions",
            organizations_url = "https://api.github.com/users/alnaimi-github/orgs",
            repos_url = "https://api.github.com/users/alnaimi-github/repos",
            events_url = "https://api.github.com/users/alnaimi-github/events{/privacy}",
            received_events_url = "https://api.github.com/users/alnaimi-github/received_events",
            type = "User",
            site_admin = false,
            name = "Abdulwaisa Al Nuaimi",
            company = (string)null,
            blog = "",
            location = "Yemen, Sana'a",
            email = (string)null,
            hireable = true,
            bio = "“Software Developer | Junior .NET Developer | Full Stack Developer”",
            twitter_username = (string)null,
            public_repos = 34,
            public_gists = 1,
            followers = 8,
            following = 31,
            created_at = "2023-06-18T13:11:21Z",
            updated_at = "2024-09-26T22:59:07Z"
        })
        .WithHeader("content-type", "application/json,charset=utf-8")
        .WithStatusCode(200));


Console.WriteLine("Press any key to stop the server...");
Console.ReadKey();

wireMockServer.Stop();
wireMockServer.Dispose();