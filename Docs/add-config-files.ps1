# PowerShell script to add appsettings files to csproj

$csprojFile = "PropMT5ConnectionService.csproj"

# Read the current content
$xml = [xml](Get-Content $csprojFile)

# Create new ItemGroup for configuration files
$itemGroup = $xml.CreateElement("ItemGroup", $xml.Project.NamespaceURI)

# Add appsettings.Development.json
$noneElementDev = $xml.CreateElement("None", $xml.Project.NamespaceURI)
$noneElementDev.SetAttribute("Include", "appsettings.Development.json")
$copyToOutputDev = $xml.CreateElement("CopyToOutputDirectory", $xml.Project.NamespaceURI)
$copyToOutputDev.InnerText = "PreserveNewest"
$noneElementDev.AppendChild($copyToOutputDev) | Out-Null
$itemGroup.AppendChild($noneElementDev) | Out-Null

# Add appsettings.Production.json
$noneElementProd = $xml.CreateElement("None", $xml.Project.NamespaceURI)
$noneElementProd.SetAttribute("Include", "appsettings.Production.json")
$copyToOutputProd = $xml.CreateElement("CopyToOutputDirectory", $xml.Project.NamespaceURI)
$copyToOutputProd.InnerText = "PreserveNewest"
$noneElementProd.AppendChild($copyToOutputProd) | Out-Null
$itemGroup.AppendChild($noneElementProd) | Out-Null

# Add the ItemGroup to the project
$xml.Project.AppendChild($itemGroup) | Out-Null

# Save the file
$xml.Save((Resolve-Path $csprojFile))

Write-Host "? Successfully added appsettings files to project file"
Write-Host "?? Files will now be copied to output directory on build"
