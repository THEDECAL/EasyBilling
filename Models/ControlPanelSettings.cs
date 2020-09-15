using EasyBilling.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyBilling.Models
{
    public class ControlPanelSettings
    {
        const int MAX_PAGE_ROWS_CNT = 100;

        private string _sortField = "Id";
        private int _pageRowsCount = 10;
        private int _currentPage = 1;
        private string _searchText = "";
        private string _filterField = "";

        public string SortField
        {
            get => _sortField;
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _sortField = value;
                }
            }
        }
        public SortType SortType { get; set; } = SortType.ASC;
        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                if (value > 0)
                {
                    _currentPage = value;
                }
            }
        }
        public int PageRowsCount
        {
            get => _pageRowsCount;
            set
            {
                if (value > 0 && value <= MAX_PAGE_ROWS_CNT)
                {
                    _pageRowsCount = value;
                }
            }
        }
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _searchText = value.Trim().ToLower();
                }
            }
        }
        public string FilterField
        {
            get => _filterField;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    _filterField = value;
                }
            }
        }
    }
}
