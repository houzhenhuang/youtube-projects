# 获取所有模板目录
$path = pwd;
$templates = Get-ChildItem -Directory $efcore_path

# 定义生成 Nupkg 包
function generate($path, $templates){
    $dir = $path.Path;
    Write-Warning "正在生成 [$dir] Nupkg 包......";

    # 遍历所有模板
    for ($i = 0; $i -le $templates.Length - 1; $i++){
        $item = $templates[$i];

        # 获取完整路径
        $fullName = $item.FullName;

        Write-Output "-----------------";
        $fullName
        Write-Output "-----------------";

        # 生成 .nupkg 包
        .\nuget.exe pack $fullName;
    }

    Write-Output "生成成功";
}

# 生成 Nupkg 包
generate -path $path -templates $templates