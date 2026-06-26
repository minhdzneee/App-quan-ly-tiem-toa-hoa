using System.Collections.ObjectModel;
using System.Windows.Input;
using GroceryStoreManagement.BLL.Services;
using GroceryStoreManagement.Models;
using GroceryStoreManagement.Presentation.Commands;

namespace GroceryStoreManagement.Presentation.ViewModels;

public class SanPhamViewModel : BaseViewModel
{
    private readonly SanPhamService _sanPhamService;
    private readonly DanhMucService _danhMucService;
    private readonly DonViTinhService _donViTinhService;
    private string _searchKeyword = string.Empty;
    private DanhMuc? _selectedDanhMucFilter;
    private SanPham? _selectedSanPham;
    private SanPham _editingSanPham = new();
    private string _statusMessage = string.Empty;

    public SanPhamViewModel(SanPhamService sanPhamService, DanhMucService danhMucService, DonViTinhService donViTinhService)
    {
        _sanPhamService = sanPhamService;
        _danhMucService = danhMucService;
        _donViTinhService = donViTinhService;

        LoadCommand = new AsyncRelayCommand(_ => LoadAsync());
        SearchCommand = new AsyncRelayCommand(_ => SearchAsync());
        NewCommand = new RelayCommand(_ => NewSanPham());
        SaveCommand = new AsyncRelayCommand(_ => SaveAsync());
        DeleteCommand = new AsyncRelayCommand(_ => DeleteAsync(), _ => SelectedSanPham is not null);

        LoadCommand.Execute(null);
    }

    public ObservableCollection<SanPham> SanPhams { get; } = new();
    public ObservableCollection<DanhMuc> DanhMucs { get; } = new();
    public ObservableCollection<DonViTinh> DonViTinhs { get; } = new();

    public string SearchKeyword
    {
        get => _searchKeyword;
        set => SetProperty(ref _searchKeyword, value);
    }

    public DanhMuc? SelectedDanhMucFilter
    {
        get => _selectedDanhMucFilter;
        set => SetProperty(ref _selectedDanhMucFilter, value);
    }

    public SanPham? SelectedSanPham
    {
        get => _selectedSanPham;
        set
        {
            if (SetProperty(ref _selectedSanPham, value) && value is not null)
            {
                EditingSanPham = Clone(value);
            }

            DeleteCommand.RaiseCanExecuteChanged();
        }
    }

    public SanPham EditingSanPham
    {
        get => _editingSanPham;
        set => SetProperty(ref _editingSanPham, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public ICommand LoadCommand { get; }
    public ICommand SearchCommand { get; }
    public ICommand NewCommand { get; }
    public ICommand SaveCommand { get; }
    public AsyncRelayCommand DeleteCommand { get; }

    private async Task LoadAsync()
    {
        try
        {
            DanhMucs.Clear();
            foreach (var item in await _danhMucService.GetAllAsync())
            {
                DanhMucs.Add(item);
            }

            DonViTinhs.Clear();
            foreach (var item in await _donViTinhService.GetAllAsync())
            {
                DonViTinhs.Add(item);
            }

            await SearchAsync(updateStatus: true);
            NewSanPham(updateStatus: false);
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
    }

    private async Task SearchAsync(bool updateStatus = true)
    {
        try
        {
            SanPhams.Clear();
            var products = await _sanPhamService.SearchAsync(SearchKeyword, SelectedDanhMucFilter?.Id);
            foreach (var product in products)
            {
                SanPhams.Add(product);
            }

            if (updateStatus)
            {
                StatusMessage = $"Tìm thấy {SanPhams.Count} sản phẩm.";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
    }

    private void NewSanPham(bool updateStatus = true)
    {
        EditingSanPham = new SanPham
        {
            DanhMucId = DanhMucs.FirstOrDefault()?.Id ?? 0,
            DonViTinhId = DonViTinhs.FirstOrDefault()?.Id ?? 0,
            TonToiThieu = 5,
            TrangThai = "DangBan"
        };
        SelectedSanPham = null;
        if (updateStatus)
        {
            StatusMessage = "Đang thêm sản phẩm mới.";
        }
    }

    private async Task SaveAsync()
    {
        try
        {
            if (EditingSanPham.Id == 0)
            {
                var newId = await _sanPhamService.AddAsync(EditingSanPham);
                var savedProduct = await _sanPhamService.GetByIdAsync(newId);
                if (savedProduct is null)
                {
                    throw new InvalidOperationException("Da them san pham nhung khong doc lai duoc tu co so du lieu.");
                }

                StatusMessage = $"Đã thêm sản phẩm vào cơ sở dữ liệu. Id: {newId}.";
            }
            else
            {
                await _sanPhamService.UpdateAsync(EditingSanPham);
                StatusMessage = "Đã cập nhật sản phẩm trong cơ sở dữ liệu.";
            }

            var successMessage = StatusMessage;
            await SearchAsync(updateStatus: false);
            NewSanPham(updateStatus: false);
            StatusMessage = successMessage;
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
    }

    private async Task DeleteAsync()
    {
        if (SelectedSanPham is null)
        {
            return;
        }

        try
        {
            await _sanPhamService.DeleteAsync(SelectedSanPham.Id);
            var successMessage = "Đã xóa sản phẩm.";
            await SearchAsync(updateStatus: false);
            NewSanPham(updateStatus: false);
            StatusMessage = successMessage;
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
    }

    private static SanPham Clone(SanPham product)
    {
        return new SanPham
        {
            Id = product.Id,
            MaSanPham = product.MaSanPham,
            TenSanPham = product.TenSanPham,
            DanhMucId = product.DanhMucId,
            TenDanhMuc = product.TenDanhMuc,
            DonViTinhId = product.DonViTinhId,
            TenDonVi = product.TenDonVi,
            GiaNhap = product.GiaNhap,
            GiaBan = product.GiaBan,
            SoLuongTon = product.SoLuongTon,
            TonToiThieu = product.TonToiThieu,
            HanSuDung = product.HanSuDung,
            MaVach = product.MaVach,
            TrangThai = product.TrangThai
        };
    }
}
