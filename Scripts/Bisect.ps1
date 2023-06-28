param (
    [Parameter(Mandatory=$true)][string]$path,
    [Parameter(Mandatory=$true)][string]$build,
	[Parameter(Mandatory=$true)][string]$badhash,
    [Parameter(Mandatory=$true)][string]$goodhash
 )

$og_path = $path
$user = $Env:Username
$branch = "upgrade-$build"
if ( git rev-parse --verify $branch -eq 0 )
{
    $branch = "upgrade2-$build"
}

# create local branch with head at hash of bisected (potential) culprit commit
cd $path
git checkout master
git pull upstream master
git checkout -b binary-search
git checkout binary-search
git bisect start
git bisect bad $badhash
git bisect good $goodhash
git checkout -b $branch
git rev-parse HEAD
