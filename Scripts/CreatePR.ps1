param (
    [Parameter(Mandatory=$true)][string]$path,
    [Parameter(Mandatory=$true)][string]$build,
	[Parameter(Mandatory=$true)][string]$hash
 )


$user = $Env:Username
$branch = "upgrade-$build"

# make a PR
cd $path
git stage *
git commit --amend --no-edit
git push origin $branch -f
echo "https://ghe.soti.net/MobiControl/MobiControlBackend/compare/master...$($user):MobiControlBackend:$($branch)?expand=1"