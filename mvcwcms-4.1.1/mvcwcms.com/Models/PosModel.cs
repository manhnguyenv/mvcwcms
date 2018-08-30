using System;

namespace MVCwCMS.Models
{
    public class BaseObject : Object
    {
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? ModifiedBy { get; set; }
        public Guid? CreatedBy { get; set; }

        public BaseObject()
        {
            CreatedDate = DateTime.Now;
        }
    }

    //
    // Order: Bán hàng
    //
    public class Order : BaseObject
    {
        public Order()
        {
        }
    }

    //
    // Purchase: Nhập hàng
    //
    public class Purchase : BaseObject
    {
        public Purchase()
        {
        }
    }

    //
    // Inventory: Kho hàng (Hàng tồn)
    //
    public class Inventory : BaseObject
    {
        public Inventory()
        {
        }
    }

    //
    // 1. Product: Hàng hóa (Sản phẩm)
    //
    public class Product : BaseObject
    {
        public int Id { get; set; }             // Khóa chính
        public string Name { get; set; }        // Tên sản phẩm
        public string Code { get; set; }        // Mã sản phẩm (tìm kiếm nhanh)
        public string Description { get; set; } // Miêu tả sản phẩm
        public string ImageUrl { get; set; }    // Đường dẫn tương đối đến ảnh sản phẩm
        public decimal? Price { get; set; }      // Giá sản phẩm
        public int? CategoryId { get; set; }     // Nhóm sản phẩm

        public Product()
        {
        }
    }

    //
    // 2. ProductCategory: Nhóm sản phẩm
    //

    public class ProductCategory : BaseObject
    {
        public int Id { get; set; }                 // Khóa chính
        public string Name { get; set; }            // Tên nhóm sản phẩm
        public string Description { get; set; }     // Miêu tả nhóm sản phẩm
        public int? ParentId { get; set; }          // Khóa của nhóm sản phẩm cha

        public ProductCategory()
        {
        }
    }

    //
    // 3. Store: Cửa hàng
    //
    public class Store : BaseObject
    {
        public int Id { get; set; }                 // Khóa chính
        public string Name { get; set; }            // Tên cửa hàng
        public string Description { get; set; }     // Miêu tả cửa hàng
        public string Address { get; set; }         // Địa chỉ cửa hàng
        public string Phone { get; set; }           // Điện thoại cửa hàng
        public int Owner { get; set; }              // Ai là chủ cửa hàng?

        public Store()
        {
        }
    }

    //
    // 4. StoreUser: Người dùng
    //
    public class StoreUser : BaseObject
    {
        public int Id { get; set; }                 // Khóa chính
        public string Name { get; set; }            // Tên người dùng
        public string UserName { get; set; }        // Mã người dùng
        public string Description { get; set; }     // Miêu tả người dùng
        public string Address { get; set; }         // Địa chỉ người dùng
        public string Phone { get; set; }           // Điện thoại người dùng
        public int Team { get; set; }               // Thuộc nhóm của ai?

        public StoreUser()
        {
        }
    }

    //
    // 5. Team: Đội nhóm
    //
    public class Team : BaseObject
    {
        public int Id { get; set; }                 // Khóa chính
        public string Name { get; set; }            // Tên nhóm
        public string Slogan { get; set; }          // Khẩu hiểu của nhóm
        public string Description { get; set; }     // Miêu tả nhóm

        public Team()
        {
        }
    }

    //
    // Promotion: Chương trình khuyến mại
    //
    public class Promotion : BaseObject
    {
        public Promotion()
        {
        }
    }

    //
    // 6. Customer: Khách hàng
    //
    public class Customer : BaseObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }
        public string Facebook { get; set; }

        public string Zalo { get; set; }

        public int? CustomerGroupId { get; set; }

        public Customer()
        {
        }
    }

    //
    // 7. CustomerGroup: Nhóm khách hàng
    //
    public class CustomerGroup : BaseObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }

        public CustomerGroup()
        {
        }
    }

    //
    // 8. Discount: Chiết khấu
    //
    public class Discount : BaseObject
    {
        public int Id { get; set; }                 // Khóa chính
        public int CustomerId { get; set; }         // Khách hàng
        public string Level { get; set; }           // Cấp
        public decimal? Sale { get; set; }          // Doanh số
        public decimal? CK { get; set; }            // Chiết khấu
        public decimal? Capital { get; set; }       // Vốn
        public decimal? Profit { get; set; }        // Lợi nhuận
        public decimal? Bonus { get; set; }         // Thưởng
        public decimal? Salary { get; set; }        // Lương
        public decimal? TotalProfit { get; set; }   // Tổng lợi nhuận
        public bool IsActive { get; set; }          // Hoạt động
    }

    //
    // Supplier: Nhà cung cấp
    //
    public class Supplier : BaseObject
    {
        public Supplier()
        {
        }
    }

    //
    // Income: Phiếu thu
    //
    public class Income : BaseObject
    {
        public Income()
        {
        }
    }

    //
    // Outcome: Phiếu chi
    //
    public class Outcome : BaseObject
    {
        public Outcome()
        {
        }
    }

    //
    // Ledger: Sổ cái
    //
    public class Ledger : BaseObject
    {
        public Ledger()
        {
        }
    }

    //
    // Revenues: Báo cáo
    //
    public class Revenues : BaseObject
    {
        public Revenues()
        {
        }
    }

    //
    // Employee: Nhân viên
    //
    public class Employee : BaseObject
    {
        public Employee()
        {
        }
    }

    //
    // Setting: Thiết lập
    //
    public class Setting : BaseObject
    {
        public Setting()
        {
        }
    }

    //
    // Vai trò của người dùng trong tổ chức
    //
    public enum OrganizationRole
    {
        CN1,                // Chi nhánh 2
        CN2,                // Chi nhánh 1
        Tier2,              // Đại lý cấp 2
        Tier1,              // Đại lý cấp 1
        Agency,             // Tổng đại lý
        NPP2,               // Nhà phân phối 2
        NPP1,               // Nhà phân phối 1
        Director,           // Giám đốc kinh doanh
        BigBoss,            // Tổng giám đốc
    }

    //
    // Vai trò của người dùng đối với phần mềm bán hàng
    //
    public enum UserRole
    {
        Boss,       // Chủ cửa hàng
        Manager,    // Quản lý
        Staff,      // Nhân viên kho
        Cashier,    // Nhân viên thu ngân
        Salesman    // Nhân viên bán hàng
    }

    //
    // Hình Thức Thanh Toán: (Tiền mặt | Thẻ | Chuyển khoản)
    //
    public enum PaymentMethod
    {
        Cash,       // Tiền mặt
        Card,       // Thẻ
        Transfer    // Chuyển khoản
    }

    //
    // Trạng thái đơn hàng: Lưu tạm | Hoàn thành
    //
    public enum OrderStatus
    {
        Draft,      // Lưu tạm
        Done,       // Hoàn thành
    }

    //
    // Những chuỗi được định nghĩa sẵn
    //
    public class CommonString
    {
        public static string Title = "Lamer Care";
        public static string TitleVN = "Mỹ phẩm xanh cho làn da đẹp";
    }
}