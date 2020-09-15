using EasyBilling.Attributes;
using EasyBilling.Data;
using EasyBilling.HtmlHelpers;
using EasyBilling.Models;
using EasyBilling.Models.Pocos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace EasyBilling.ViewModels
{
    [Flags]
    public enum SortType { ASC, DESC }
    public class DataViewModel<T> where T : class
    {
        private readonly DbSet<T> _dbSet;
        private Type _type;
        private Func<T, bool> _filter;
        public TableHtmlHelper<T> TableHelper { get; private set; }
        public string UrlPath { get; }
        public ControlPanelSettings Settings { get; private set; }
        public string[] IncludeFields { get; private set; }
        public string[] ExcludeFields { get; private set; }
        public List<T> Data { get; private set; }
        public int AmountPage { get; private set; }
        public bool IsHaveNextPage { get => ( Settings.CurrentPage + 1 > AmountPage) ? false : true; }
        public bool IsHavePreviousPage { get => ( Settings.CurrentPage - 1 < 1) ? false : true; }
        public int RowsCount { get; set; }

        public DataViewModel(IServiceScopeFactory scopeFactory,
            ControlPanelSettings settings,
            string urlPath,
            Func<T, bool> filter = null,
            string[] includeFields = null,
            string[] excludeFields = null)
        {
            var scope = scopeFactory.CreateScope();
            var sp = scope.ServiceProvider;
            var dbContext = sp.GetRequiredService<BillingDbContext>();

            _dbSet = dbContext.Set<T>();
            _type = typeof(T);
            _filter = filter;
            TableHelper = new TableHtmlHelper<T>(this);
            UrlPath = urlPath;
            Settings = settings ?? new ControlPanelSettings();
            IncludeFields = includeFields ?? new string[0];
            ExcludeFields = excludeFields ?? new string[0];
            RowsCount = _dbSet.Count();
            AmountPage = (int)Math.Ceiling(_dbSet.Count() / (double)Settings.PageRowsCount);
            Settings.CurrentPage = (Settings.CurrentPage > 0 && Settings.CurrentPage <= AmountPage)
                ? Settings.CurrentPage : 1;
            Data = new List<T>();

            GetData();
        }

        private void GetData()
        {
            var searchFunc = new Func<T, bool>((o) =>
                    o.GetType().GetProperties().Any(p => {
                        var value = p.GetValue(o, null);
                        return (value == null)
                            ? false
                            : value.ToString().ToLower().Contains(Settings.SearchText.ToLower());
                        }));
            try
            {
                IQueryable<T> queryQ = _dbSet.AsQueryable().AsNoTracking();
                if (queryQ.Count() > 0)
                {
                    //Подключение связанных объектов
                    foreach (var item in IncludeFields)
                    {
                        try
                        {
                            queryQ = queryQ.Include(item);
                        }
                        catch (Exception ex)
                        { Console.WriteLine(ex.StackTrace); }
                    }

                    IEnumerable<T> queryE = null;
                    //Выбераем поисковый запрос и фильтруем
                    try
                    {
                        queryE = queryQ.Where(searchFunc);

                        if (_filter != null)
                        {
                            queryE = queryE.Where(_filter);
                        }

                        RowsCount = queryE.Count();
                        AmountPage = (int)Math.Ceiling(queryE.Count() / (double)Settings.PageRowsCount);
                    }
                    catch (Exception ex)
                    { Console.WriteLine(ex.StackTrace); }

                    //Сортировка по выбранному столбцу
                    try
                    {
                        var prop = _type.GetProperties()
                            .FirstOrDefault(p => p.Name.ToLower()
                            .Equals(Settings.SortField.ToLower()));

                        if (Settings.SortType == SortType.DESC)
                            queryE = queryE.OrderByDescending(p =>
                                prop.GetValue(p, null)?.ToString() ?? "").ToList();
                        else
                            queryE = queryE.OrderBy(p =>
                                prop.GetValue(p, null)?.ToString() ?? "").ToList();
                    }
                    catch (Exception ex)
                    { Console.WriteLine(ex.StackTrace); }

                    //Выбераем данные для текущей страницы (пагинация)
                    try
                    {
                        Data.AddRange(queryE.Skip((Settings.CurrentPage - 1) * Settings.PageRowsCount).Take(Settings.PageRowsCount).ToList());
                    }
                    catch (Exception ex)
                    { Console.WriteLine(ex.StackTrace); }
                }
            }
            catch (Exception ex)
            { Console.WriteLine(ex.StackTrace); }
        }

        public async Task<List<Dictionary<string,string>>> GetDataDicAsync()
        {
            return await Task.Run(() => Data.Select(o =>
                {
                    Type type = o.GetType();
                    var props = type.GetProperties();
                    var dic = new Dictionary<string, string>();

                    foreach (var item in props)
                    {
                        var noShowAttr = item.GetCustomAttribute<NoShowInTableAttribute>();
                        if (!ExcludeFields.Any(f => f.Equals(item.Name)) &&
                            noShowAttr == null)
                        {
                            var val = item.GetValue(o);
                            dic.Add(item.Name, (val != null) ? val.ToString() : "");
                        }
                    }
                    return dic;
                }).ToList());
        }
    }
}
