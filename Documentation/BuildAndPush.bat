%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe /p:Configuration=Release
pushd ..\..\Ifp.Validation.wiki
git commit --all --message="XMLDoc updated"
git push
git pull
popd