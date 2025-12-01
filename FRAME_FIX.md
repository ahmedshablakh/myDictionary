# إصلاح Frame Obsolete Warning

## الحالة: ✅ تم الإصلاح

تم استبدال جميع عناصر `Frame` بـ `Border` في جميع صفحات التطبيق للتوافق مع .NET 9.

## التغييرات المطبقة:

### 1. Styles.xaml
- تم إضافة `CardBorder` Style جديد.
- تم الاحتفاظ بـ `CardFrame` كمرجع قديم (يمكن حذفه لاحقاً).

### 2. الصفحات المحدثة:
- ✅ `DashboardPage.xaml`
- ✅ `FlashcardsPage.xaml`
- ✅ `TestsPage.xaml`
- ✅ `WordsPage.xaml`

## ملاحظات:
- `Border` يوفر أداء أفضل ومرونة أكبر في التصميم.
- تم استخدام `StrokeShape="RoundRectangle 16"` لمحاكاة `CornerRadius`.
- تم استخدام `Shadow` داخل `Border` لمحاكاة `HasShadow="True"`.

## التحقق:
تم بناء المشروع بنجاح (`dotnet build`) بدون أخطاء متعلقة بـ Frame.
