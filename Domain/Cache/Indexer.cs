﻿using System.Collections.Generic;
using System.Linq;

namespace Trizbort.Domain.Cache
{
  public class Indexer
  {
    public List<FindCacheItem> Index()
    {
      var items = new List<FindCacheItem>();

      foreach (var element in Project.Current.Elements.OfType<Room>())
      {
        var room = element;
        var cacheItem = new FindCacheItem
        {
          Element = room,
          Name = room.Name,
          Description = room.PrimaryDescription,
          Objects = room.Objects
        };
        items.Add(cacheItem);
      }

      return items;
    }
  }
}