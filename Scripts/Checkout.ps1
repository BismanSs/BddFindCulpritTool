param (
    [Parameter(Mandatory=$true)][string]$path,
    [Parameter(Mandatory=$true)][string]$build,
	[Parameter(Mandatory=$true)][string]$hash
 )

$og_path = $path
$user = $Env:Username
# below assumes there won't be more than 2 repeats which is generally true
$branch = "upgrade-$build"
if ( git rev-parse --verify $branch -eq 0 )
{
    $branch = "upgrade2-$build"
}

# create local branch with head at hash of potential culprit commit
cd $path
git checkout master
git pull upstream master
git checkout -b $branch
git reset --hard $hash
git commit --amend --no-edit

