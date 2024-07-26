# Sử dụng hình ảnh .NET SDK để xây dựng ứng dụng
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

# Đặt thư mục làm việc
WORKDIR /app

# Sao chép tệp csproj vào thư mục làm việc
COPY Pizza.csproj ./

# Khôi phục các phụ thuộc
RUN dotnet restore

# Sao chép tất cả các tệp vào thư mục làm việc
COPY . ./

# Xây dựng ứng dụng
RUN dotnet publish -c Release -o out

# Sử dụng hình ảnh .NET Runtime để chạy ứng dụng
FROM mcr.microsoft.com/dotnet/aspnet:7.0

# Đặt thư mục làm việc
WORKDIR /app

# Sao chép ứng dụng đã xây dựng từ bước trước
COPY --from=build /app/out ./

# Cấu hình cổng mà ứng dụng sẽ lắng nghe
EXPOSE 80

# Chạy ứng dụng
ENTRYPOINT ["dotnet", "Pizza.dll"]
