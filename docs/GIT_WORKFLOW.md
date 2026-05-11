# Git Workflow cho Nhóm 7 - CLB Võ Thuật

## 🌳 Chiến Lược Branching

Chúng tôi sử dụng **Git Flow** được điều chỉnh cho phù hợp với dự án nhỏ và team sinh viên.

### 📋 Các Branch Chính

#### 1. **main** (Production)
- **Mục đích**: Chứa code production-ready
- **Quy tắc**: 
  - Luôn stable và có thể deploy
  - Chỉ merge từ `develop` qua Pull Request
  - Tên branch: `main`

#### 2. **develop** (Development)
- **Mục đích**: Branch phát triển chính
- **Quy tắc**:
  - Chứa code mới nhất đã được review
  - Merge từ feature branches
  - Tên branch: `develop`

#### 3. **feature/*** (Tính năng mới)
- **Mục đích**: Phát triển tính năng cụ thể
- **Quy tắc**:
  - Tạo từ `develop`
  - Tên branch: `feature/ten-tinh-nang`
  - Ví dụ: `feature/quan-ly-hoc-vien`, `feature/api-authentication`

#### 4. **hotfix/*** (Sửa lỗi gấp)
- **Mục đích**: Sửa lỗi production khẩn cấp
- **Quy tắc**:
  - Tạo từ `main`
  - Merge vào cả `main` và `develop`
  - Tên branch: `hotfix/fix-loi-xyz`

#### 5. **release/*** (Chuẩn bị release)
- **Mục đích**: Chuẩn bị phiên bản mới
- **Quy tắc**:
  - Tạo từ `develop`
  - Chỉ sửa lỗi, không thêm feature
  - Tên branch: `release/v1.0.0`

## 🔄 Quy Trình Work

### 🚀 Làm Feature Mới

1. **Bắt đầu feature**
   ```bash
   git checkout develop
   git pull origin develop
   git checkout -b feature/ten-feature
   ```

2. **Làm việc**
   - Commit thường xuyên với message rõ ràng
   - Push lên remote branch hàng ngày
   ```bash
   git add .
   git commit -m "feat: them API quan ly hoc vien"
   git push origin feature/ten-feature
   ```

3. **Hoàn thành feature**
   ```bash
   git checkout develop
   git pull origin develop
   git merge feature/ten-feature --no-ff
   git push origin develop
   git branch -d feature/ten-feature
   ```

### 🔧 Sửa Lỗi

#### Lỗi thường (không gấp)
```bash
git checkout develop
git pull origin develop
git checkout -b fix/bug-loi-xyz
# ... sửa lỗi ...
git checkout develop
git merge fix/bug-loi-xyz --no-ff
git push origin develop
```

#### Lỗi gấp (production)
```bash
git checkout main
git pull origin main
git checkout -b hotfix/fix-loi-xyz
# ... sửa lỗi ...
git checkout main
git merge hotfix/fix-loi-xyz --no-ff
git tag -a v1.0.1 -m "Fix loi xyz"
git push origin main --tags

# Merge vào develop
git checkout develop
git merge hotfix/fix-loi-xyz --no-ff
git push origin develop
```

### 📦 Release

1. **Tạo release branch**
   ```bash
   git checkout develop
   git pull origin develop
   git checkout -b release/v1.0.0
   ```

2. **Hoàn thiện release**
   - Cập nhật version number
   - Fix bugs cuối cùng
   - Cập nhật documentation

3. **Merge và tag**
   ```bash
   # Merge vào main
   git checkout main
   git merge release/v1.0.0 --no-ff
   git tag -a v1.0.0 -m "Release version 1.0.0"
   git push origin main --tags
   
   # Merge vào develop
   git checkout develop
   git merge release/v1.0.0 --no-ff
   git push origin develop
   
   # Xóa release branch
   git branch -d release/v1.0.0
   ```

## 📝 Quy Tắc Commit Message

Sử dụng **Conventional Commits**:

```
<týp>: <mô tả ngắn>

[optional body]

[optional footer]
```

### Các loại (type):
- `feat`: Feature mới
- `fix`: Bug fix
- `docs`: Documentation
- `style`: Code style (formatting, missing semicolon...)
- `refactor`: Code refactor
- `test`: Thêm/sửa test
- `chore`: Build process, dependency management

### Ví dụ:
```bash
feat: them API quan ly hoc vien
fix: loi trang dashboard khi load data
docs: cap nhat README.md
refactor: tach file UserService.cs
test: them unit test cho HocVienController
```

## 🤝 Quy Trình Pull Request

### Khi tạo PR:
1. **Title**: Rõ ràng, theo format commit message
2. **Description**: 
   - Mục đích của PR
   - Các thay đổi chính
   - Cách test (nếu có)
3. **Assignees**: Gán cho reviewer
4. **Labels**: Đánh loại PR (feature, bug, hotfix)

### Review Process:
1. **Tối thiểu 1 reviewer** approve
2. **Tất cả tests phải pass**
3. **Không có conflicts**
4. **Code phải tuân thủ coding standards**

### Merge Rules:
- **Squash merge** cho feature branches
- **Merge commit** cho release/hotfix branches
- **Không bao giờ force push** vào main/develop

## 👥 Phân Công Nhóm

### Vai trò:
- **Lead Developer**: Quản lý main branch, approve PR
- **Backend Developers**: Làm feature branches, review code
- **Frontend Developers**: Làm feature branches, review code

### Quy trình:
1. **Daily standup**: Cập nhật tiến độ
2. **Weekly planning**: Chia task cho tuần
3. **Code review**: Thực hiện qua Pull Request
4. **Integration**: Merge vào develop hàng ngày

## 🛠️ Tools & Commands

### Commands hữu ích:
```bash
# Xem tất cả branches
git branch -a

# Xem commit history
git log --oneline --graph --all

# Xóa local branch đã merge
git branch -d feature/ten-feature

# Xóa remote branch
git push origin --delete feature/ten-feature

# Stash changes tạm thời
git stash
git stash pop

# Resolve conflicts
git merge --abort
```

### Setup cho lần đầu:
```bash
# Config user
git config --global user.name "Tên của bạn"
git config --global user.email "email@example.com"

# Setup aliases (tùy chọn)
git config --global alias.co checkout
git config --global alias.br branch
git config --global alias.ci commit
git config --global alias.st status
```

## 📋 Checklist Trước Khi Merge

- [ ] Code đã được test kỹ
- [ ] Tất cả tests pass
- [ ] Documentation đã cập nhật
- [ ] Không có console.log/debug code
- [ ] Code follows team conventions
- [ ] PR description rõ ràng
- [ ] Reviewer đã approve
- [ ] Không có merge conflicts

## 🚨 Troubleshooting

### Merge conflicts:
```bash
git checkout develop
git pull origin develop
git merge feature/ten-feature
# ... resolve conflicts ...
git add .
git commit -m "resolve: merge conflicts in feature/ten-feature"
```

### Force push (cẩn thận!):
```bash
# Chỉ dùng cho branch cá nhân, không bao giờ cho main/develop
git push --force-with-lease origin feature/ten-feature
```

### Revert commit:
```bash
git revert <commit-hash>
git push origin develop
```

---

**Lưu ý quan trọng**: 
- Luôn pull trước khi push
- Không bao giờ làm việc trực tiếp trên main/develop
- Commit message phải rõ ràng và nhất quán
- Test kỹ trước khi merge
