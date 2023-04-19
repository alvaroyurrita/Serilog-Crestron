param ($ProjectDir, $OutDir, $ProjectName, $Configuration)

function SendToProc{
    Invoke-Expression "echo y | pscp -l $($Env.Processor.UserName) -pw $($Env.Processor.Password) -sftp ""$ProjectDir$OutDir$ProjectName.cpz"" $($Env.Processor.Address):program$($Env.Processor.SlotNo)" 
}

function SendToVC4{
    Invoke-Expression "curl.exe -X PUT https://$($Env.Virtual.Address)/VirtualControl/config/api/ProgramLibrary -H Authorization:$($Env.Virtual.Token) -H Content-Type:multipart/form-data -F ""ProgramId=$($Env.Virtual.ProgramId)"" -F ""AppFile=@$ProjectDir$OutDir$ProjectName.cpz"" -F ""StartNow=true"" -k"
}

If (-not(Test-Path ".ENV")){
    return;
}
$Env = Get-Content -Raw ".ENV" | ConvertFrom-JSON
Write-Host "Sending to $($Env.Processor.Address) For $Configuration"
switch ($Configuration){
    "Debug"{
        SendToProc
        Invoke-Expression "plink -pw $($Env.Processor.Password) $($Env.Processor.UserName)@$($Env.Processor.Address) -batch ""PROGLOAD -P:$($Env.Processor.SlotNo) -D""" 
        Invoke-Expression "plink -pw $($Env.Processor.Password) $($Env.Processor.UserName)@$($Env.Processor.Address) -batch ""DEBUGPROGRAM -P:$($Env.Processor.SlotNo) -Port:50000 -IP:0.0.0.0 -S""" 
        Invoke-Expression "plink -pw $($Env.Processor.Password) $($Env.Processor.UserName)@$($Env.Processor.Address) -batch ""PROGRESET -P:$($Env.Processor.SlotNo)""" 
    }
    "Release" {
        SendToProc
        Invoke-Expression "plink -pw $($Env.Processor.Password) $($Env.Processor.UserName)@$($Env.Processor.Address) -batch ""PROGLOAD -P:$($Env.Processor.SlotNo)""" 
    }
    "DebugVC4"{
        SendToVC4
    }
    "ReleaseVC4"{
        SendToVC4
    }
    Default {
    }
}

<#  enviromental file
.ENV Properties
 {
    "Processor" : 
    {
        "UserName":"",
        "Password":"",
        "Address":"",
        "SlotNo":""
    },
    "Virtual":
    {
        "Token":"",
        "Address":"",
        "ProgramId":""
    }
 }
#>

<# MSBUILD Addition: Add at the end inside the Project Tag
    <Target Name="CLZPostBuildDebug" AfterTargets="SimplSharpPostProcess" Condition="'$(Configuration)'=='Debug'">
        <Exec Command="powershell.exe –command &quot;&amp; { .\Deployment.ps1 '$(ProjectDir)' '$(OutDir)' '$(ProjectName)' '$(Configuration)' }&quot;" />
    </Target>
    <Target Name="CLZPostBuildRelease" AfterTargets="SimplSharpPostProcess" Condition="'$(Configuration)'=='Release'">
        <Exec Command="powershell.exe –command &quot;&amp; { .\Deployment.ps1 '$(ProjectDir)' '$(OutDir)' '$(ProjectName)' '$(Configuration)' }&quot;" />
    </Target>
    <Target Name="CLZPostBuildReleaseVC4" AfterTargets="SimplSharpPostProcess" Condition="'$(Configuration)'=='ReleaseVC4'">
        <Exec Command="powershell.exe –command &quot;&amp; { .\Deployment.ps1 '$(ProjectDir)' '$(OutDir)' '$(ProjectName)' '$(Configuration)' }&quot;" />
    </Target>
    <Target Name="CLZPostBuildReleaseNoPush" AfterTargets="SimplSharpPostProcess" Condition="'$(Configuration)'=='ReleaseNoPush'">
        <Message Text="Release Compiling Only. No Processor communication" Importance="High" />
        <Copy SourceFiles="$(ProjectDir)$(OutDir)$(ProjectName).cpz" DestinationFolder="../" />
    </Target>

#>
