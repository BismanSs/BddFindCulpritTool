param (
    [Parameter(Mandatory=$true)][string]$path,
    [Parameter(Mandatory=$true)][string]$build,
	[Parameter(Mandatory=$true)][string]$hash
 )

$og_path = $path
$user = $Env:Username
# below assumes there won't be more than 2 repeats which is generally true
$branch = "upgrade-$build"

# make a PR
cd $path
git stage *
git commit --amend --no-edit
git push origin $branch -f