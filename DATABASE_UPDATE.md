# تحديثات قاعدة البيانات - إضافة ExampleTranslation

## Migration المطلوب

تم إضافة حقل `ExampleTranslation` إلى جدول `Words`.

### للتطبيق على قاعدة البيانات:

#### لـ Web Application (SQL Server):
```bash
cd src/MyDictionary.Web
dotnet ef migrations add AddExampleTranslation
dotnet ef database update
```

#### لـ Mobile Application (SQLite):
القاعدة ستُحدث تلقائياً عند تشغيل التطبيق لأول مرة بعد التحديث.

## الحقول المضافة:
- `ExampleTranslation` (string, nullable) - ترجمة الجملة المثالية
- `SourceLanguage` (computed property) - اسم مستعار لـ LanguageFrom
- `TargetLanguage` (computed property) - اسم مستعار لـ LanguageTo
