"use client";

import { useEffect, useState } from "react";
import { useSearchParams } from "next/navigation";
import {
    CircularProgress,
    Container,
    Typography,
    Box,
    Paper,
} from "@mui/material";
import CheckCircleOutlineIcon from "@mui/icons-material/CheckCircleOutline";
import ErrorOutlineIcon from "@mui/icons-material/ErrorOutline";
import axios from "axios";

type EmailVerifyResponse = {
    success: boolean;
    traceId?: string;
    message?: string;
    error?: {
        code?: number;
        details?: string;
    };
};

export default function EmailVerifyPage() {
    const searchParams = useSearchParams();
    const [status, setStatus] = useState<"pending" | "success" | "error">("pending");
    const [message, setMessage] = useState<string>("正在激活您的账户，请稍候...");

    useEffect(() => {
        const token = searchParams.get("token");
        const locale = searchParams.get("locale") ?? "en";

        if (!token) {
            setStatus("error");
            setMessage("链接无效或已过期。");
            return;
        }

        const activate = async () => {
            try {
                const backendUrl = process.env.NEXT_PUBLIC_BACKEND_SERVER_URL;

                const res = await axios.post<EmailVerifyResponse>(
                    `${backendUrl}/v1/auth/emails/verify`,
                    { token },
                    {
                        headers: {
                            "X-Locale": locale,
                        },
                        validateStatus: () => true, // 👈 避免 axios 抛异常
                    }
                );

                if (res.data.success) {
                    setStatus("success");
                    setMessage(res.data.message ?? "您的账户已成功激活！");
                } else {
                    setStatus("error");
                    setMessage(res.data.error?.details ?? "激活失败，请稍后重试。");
                    console.error("Email verification failed:", res.data);
                }
            } catch (error) {
                console.error("Email verification error:", error);
                setStatus("error");
                setMessage("激活请求异常，请稍后再试。");
            }
        };

        activate();
    }, [searchParams]);

    return (
        <Container maxWidth="sm" sx={{ mt: 10 }}>
            <Paper elevation={3} sx={{ p: 4, textAlign: "center" }}>
                <Box sx={{ mb: 3 }}>
                    {status === "pending" && <CircularProgress size={60} />}
                    {status === "success" && (
                        <CheckCircleOutlineIcon color="success" sx={{ fontSize: 60 }} />
                    )}
                    {status === "error" && (
                        <ErrorOutlineIcon color="error" sx={{ fontSize: 60 }} />
                    )}
                </Box>
                <Typography variant="h6">{message}</Typography>
            </Paper>
        </Container>
    );
}