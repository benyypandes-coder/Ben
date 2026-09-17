# Kurt Dhylan Moto Shop Inventory V3

MAUI Android inventory starter project.

Functions included:
- Add Products: name, brand, model, price, stocks
- Add Services: labor/service name and price
- Search Products
- Search Services
- Remove products/services
- GitHub Actions APK build workflow

## GitHub ZIP workflow

The included workflow can find `Ben.zip` in the repository (root or one level below), extract it, locate the `.csproj`, restore for `net10.0-android`, publish an APK, and upload the APK as a GitHub Actions artifact.

For the workflow itself to run, `.github/workflows/build-android.yml` must exist in the repository outside the ZIP.
