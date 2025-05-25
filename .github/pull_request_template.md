<!-- Please fill out the checklist below and link relevant tasks/issues -->

### Feat: Stay Signed in

#### ✅ Completed the following features:

- [ ] Imported an error filter into `app.module.ts`
- [ ] Added `AuthModule` into `ExchangeKeyModule` and `ExchangeKeyModule`
- [ ] Changed `GqlAuthGuard` to `CombinedAuthGuard` at resolver in `ExchangeKeyResolver`, `UserExchangeResolver`
- [ ] Modified the `generateAccessToken` and `revokeTokens` in `auth.service.ts`
- [ ] Added `generateAccessToken` method into `auth.resolver.ts`
- [ ] Exported `EmailVerificationService` at `auth.module.ts`
- [ ] Modified `combined-auth.guard.spec.ts`
- [ ] Changed the error from `UnauthorizedException` to `CustomException` to add response code
- [ ] Deleted `GqlAuthGuard`
- [ ] Added `GqlHttpExceptionFilter`
- [ ] Added `CustomException`
- [ ] Added `ACCESS_TOKENS_NOT_SAME` and `BOTH_TOKENS_INVALID` into `code.ts`

---

#### 🧩 Related Modules:
- Auth
- ExchangeKey
- User

---

#### 📌 Resolve:
CP-20