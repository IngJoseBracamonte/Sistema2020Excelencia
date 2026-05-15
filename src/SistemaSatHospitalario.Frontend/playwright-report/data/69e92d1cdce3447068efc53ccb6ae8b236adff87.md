# Instructions

- Following Playwright test failed.
- Explain why, be concise, respect Playwright best practices.
- Provide a snippet of code with the fix, if possible.

# Test info

- Name: health.spec.ts >> API Health check
- Location: e2e\health.spec.ts:17:5

# Error details

```
Error: apiRequestContext.get: connect ECONNREFUSED ::1:5000
Call log:
  - → GET http://localhost:5000/api/Tickets/report
    - user-agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/148.0.7778.96 Safari/537.36
    - accept: */*
    - accept-encoding: gzip,deflate,br
    - X-Testing-Token: S4T_Hosp_Testing_2026

```