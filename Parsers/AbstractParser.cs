﻿using HtmlAgilityPack;
using myYSTU.Models;
using myYSTU.Services.Http;
using System.Collections.Concurrent;

namespace myYSTU.Parsers;

public abstract class AbstractParser<T> : IParser<T>
{
    protected readonly string _linkToParse;
    protected readonly IHttpService _httpService = Application.Current.Handler.MauiContext.Services.GetService<IHttpService>();

    protected AbstractParser(string linkToParse)
    {
        _linkToParse = linkToParse;
    }

    protected virtual HttpContent GetPostContent(string date)
    {
        return null;
    }

    protected abstract IEnumerable<T> ParseHtml(HtmlDocument htmlDoc);

    public async Task<IEnumerable<T>> ParseInfo(string? postContent = null)
    {
        HttpContent? content = null;
        if (postContent is not null)
            content = GetPostContent(postContent);

        var htmlDoc = await _httpService.GetHtmlDoc(_linkToParse, content);

        return ParseHtml(htmlDoc);
    }

    public async Task<ConcurrentBag<T>> ParallelParseInfo(string? postContent = null)
    {
        HttpContent content = null;
        if (postContent is not null)
            content = GetPostContent(postContent);

        var concurrentBag = new ConcurrentBag<T>();

        var tasks = new List<Task<HtmlDocument>>();
        for (int i = 1; i <= 5; i++)
        {
            tasks.Add(_httpService.GetHtmlDoc($"{_linkToParse}{i}", content));
        }

        while (tasks.Count > 0)
        {
            var completedTask = await Task.WhenAny(tasks);

            tasks.Remove(completedTask);

            var htmlDoc = await completedTask;
            var infoList = ParseHtml(htmlDoc);

            foreach (var info in infoList)
            {
                concurrentBag.Add(info);
            }
        }
        return concurrentBag;
    }

    public Task ParseAvatarsAsync<TAvatarModel>(IEnumerable<TAvatarModel> list) where TAvatarModel : IAvatarModel
    {
        return Parallel.ForEachAsync(list, async (info, ct) =>
        {
            info.AvatarImageSource = await _httpService.GetImage(info.AvatarUrl);
        });
    }

    public Task UpdateAvatarAsync(IAvatarModel model) 
    {
        return Task.Run(async () => 
        {
            model.AvatarImageSource = await _httpService.GetImage(model.AvatarUrl);
        });
    }

    public async Task<ImageSource> GetAvatarAsync(IAvatarModel model)
    {
        return await Task.Run(() => _httpService.GetImage(model.AvatarUrl));
    }
}
